using HalloDoc_BAL.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_BAL.Repository
{


    public class JwtServices : IJwtServices
    {
        public IConfiguration Configuration { get; }
        public JwtServices(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public string GenerateToken(string username, string role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(Configuration["jwt:key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            }),
                Expires = DateTime.UtcNow.AddMinutes(Double.Parse(Configuration["jwt:Expiry"])),

                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public bool ValidateToken(string token, out JwtSecurityToken jwtToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(Configuration["jwt:key"]);


            try
            {
                // Validate the token
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero // remove delay of token when expire
                }, out SecurityToken validatedToken);

                jwtToken = (JwtSecurityToken)validatedToken;
                // If validation is successful, return true
                if(jwtToken != null)
                {
                return true;
                }
                
                return false;
            }
            catch
            {
                jwtToken = null;
                return false;
            }
        }


    }
}
