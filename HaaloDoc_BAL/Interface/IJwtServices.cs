using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;

namespace HalloDoc_BAL.Interface
{
    public interface IJwtServices
    {
        IConfiguration Configuration { get; }

        string GenerateToken(string username , string role , int? roleId);
        bool ValidateToken(string token, out JwtSecurityToken jwtToken);
    }
}