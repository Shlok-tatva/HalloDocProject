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
using HalloDoc_DAL.DataContext;

namespace HalloDoc_BAL.Repository
{

    public class CustomAuth : Attribute, IAuthorizationFilter
    {
        private readonly string[] _accountTypes;

        public CustomAuth(params string[] accountTypes)
        {
            _accountTypes = accountTypes;
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

            if (string.IsNullOrWhiteSpace(accoutType))
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Index" }));
                return;
            }

            if (_accountTypes == null || _accountTypes.Length == 0 || !_accountTypes.Contains(accoutType))
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "AccessDenied" }));
                return;
            }
        }
    }



}
