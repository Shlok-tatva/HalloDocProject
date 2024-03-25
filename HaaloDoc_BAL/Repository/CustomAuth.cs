using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using HalloDoc_BAL.Interface;
using System.IdentityModel.Tokens.Jwt;
using HalloDoc_DAL.Models;

namespace HalloDoc_BAL.Repository
{
    public class CustomAuthFilterFactory : Attribute, IFilterFactory
    {
        private readonly string _accountType;

        public CustomAuthFilterFactory(string accountType = "")
        {
            _accountType = accountType;
        }

        public bool IsReusable => false;

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            var adminFunctionRepo = serviceProvider.GetService<IAdminFunctionRepository>();
            return new CustomAuth(_accountType, adminFunctionRepo);
        }
    }

    public class CustomAuth : IAuthorizationFilter
    {
        private readonly string _accountType;
        private readonly IAdminFunctionRepository _adminFunctionRepo;

        public CustomAuth(string accountType, IAdminFunctionRepository adminFunctionRepo)
        {
            _accountType = accountType;
            _adminFunctionRepo = adminFunctionRepo;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var jwtServices = context.HttpContext.RequestServices.GetService<IJwtServices>();

            if (jwtServices == null)
            {
                return;
            }

            var token = context.HttpContext.Session.GetString("jwttoken");
            var isvalidated = jwtServices.ValidateToken(token, out JwtSecurityToken jwttoken);

            if (token == null || !isvalidated)
            {
                if (!isvalidated)
                {
                    context.HttpContext.Session.Clear();
                }

                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Index" }));
                return;
            }

          
            var accoutType = jwttoken.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
            //var roleId = jwttoken.Claims.FirstOrDefault(c => c.Type == "roleId")?.Value;

            if (string.IsNullOrWhiteSpace(accoutType))
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Index" }));
                return;
            }

            if (string.IsNullOrWhiteSpace(_accountType) || accoutType != _accountType)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "AccessDenied" }));
                return;
            }
            //var routeData = context.RouteData; // Assuming page name is controller name
            //var pageName = context.HttpContext.Request.Path.Value.Split('/').LastOrDefault();
            //if (string.IsNullOrWhiteSpace(pageName))
            //{
            //    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Error", action = "PageNotFound" }));
            //    return;
            //}

            //List<string> menuItems = GetMenuItemsForRole(roleId);
            //pageName = pageName.ToLower();

            //// Check if pageName is present in menuItems
            //if (!menuItems.Any(m => m.Equals(pageName, StringComparison.OrdinalIgnoreCase)))
            //{
            //    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "AccessDenied" }));
            //    return;
            //}

        }

        // Implement this method to retrieve menu items for the role
        private List<string> GetMenuItemsForRole(string roleid)
        {
            List<string> menuNames = new List<string>();
            List<Rolemenu> menus = _adminFunctionRepo.GetMenuByRole(Int32.Parse(roleid));
            foreach (var menu in menus)
            {
                // Get the menu name for the current menu ID
                var menuName = _adminFunctionRepo.GetMenuNameById((int)menu.Menuid);

                // Add the menu name to the list
                menuNames.Add(menuName);
            }

            // Return the list of menu names
            return menuNames;
        }
    }
}
