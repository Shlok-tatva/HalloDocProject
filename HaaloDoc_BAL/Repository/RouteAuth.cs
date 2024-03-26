﻿using HalloDoc_BAL.Interface;
using HalloDoc_DAL.DataContext;
using HalloDoc_DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_BAL.Repository
{
    public class RouteAuthFilter : Attribute, IFilterFactory
    {
       
        public bool IsReusable => false;

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetService<ApplicationDbContext>();
            return new RouteAuth(context);
        }
    }

    public class RouteAuth : IAuthorizationFilter
    {

        private readonly ApplicationDbContext _context;

        public RouteAuth(ApplicationDbContext context)
        {
            _context = context;
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

            var roleId = jwttoken.Claims.FirstOrDefault(c => c.Type == "roleId")?.Value;


            var routeData = context.RouteData; // Assuming page name is controller name
            var pageName = context.HttpContext.Request.Path.Value.Split('/').LastOrDefault();
            if (string.IsNullOrWhiteSpace(pageName))
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Error", action = "PageNotFound" }));
                return;
            }

            List<string> menuItems = GetMenuItemsForRole(roleId);
            pageName = pageName.ToLower();

            //// Check if pageName is present in menuItems
            if (!menuItems.Any(m => m.Equals(pageName, StringComparison.OrdinalIgnoreCase)))
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "AccessDenied" }));
                return;
            }

        }

        private List<string> GetMenuItemsForRole(string roleid)
        {
            List<string> menuNames = new List<string>();
            List<Rolemenu> menus = _context.Rolemenus.Where(rm => rm.Roleid == Int32.Parse(roleid)).ToList();
            foreach (var menu in menus)
            {
                var menuName = _context.Menus.Where(rm => rm.Menuid == menu.Menuid).FirstOrDefault().Name;
                menuNames.Add(menuName);
            }
            return menuNames;
        }

    }
}
