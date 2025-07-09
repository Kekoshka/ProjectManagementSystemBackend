using Microsoft.IdentityModel.Tokens;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Interfaces;
using ProjectManagementSystemBackend.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static Azure.Core.HttpHeader;

namespace ProjectManagementSystemBackend.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        IConfiguration _configuration;
        public AuthenticationService(IConfiguration configuration) 
        {
            _configuration = configuration;
        }
        public string GetJWT(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
            };

            var jwt = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: user.Id.ToString(),
                expires: DateTime.UtcNow.AddMinutes(60),
                claims: claims,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"])), SecurityAlgorithms.HmacSha256));
           
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}
