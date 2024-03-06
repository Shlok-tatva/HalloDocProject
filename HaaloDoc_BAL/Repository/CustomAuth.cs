using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using HalloDoc_BAL.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Routing;

namespace HalloDoc_BAL.Repository
{

    public class CustomAuth : Attribute, IAuthorizationFilter
    {

        private readonly string _role;

        public CustomAuth(string role = "")
        {
            _role = role;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {

            var jwtServices = context.HttpContext.RequestServices.GetService<IJwtServices>();

            if (jwtServices == null)
            {
                return;
            }

            var token = context.HttpContext.Session.GetString("jwttoken");

            if (token == null || !jwtServices.ValidateToken(token, out JwtSecurityToken jwttoken))
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Index" }));
                return;
            }

            var roleClaim = jwttoken.Claims.Where(c => c.Type == "role").FirstOrDefault();

            var roleValue = roleClaim.Value;

            if (roleClaim == null)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Index" }));
                return;
            }

            if (string.IsNullOrWhiteSpace(_role) || roleValue != _role)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Accessdenied" }));
                return;
            }
        }

    }
}
