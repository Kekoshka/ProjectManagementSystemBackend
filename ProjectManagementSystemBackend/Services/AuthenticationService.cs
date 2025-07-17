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
        
        /// <summary>
        /// Конструктор сервиса аутентификации
        /// </summary>
        /// <param name="configuration">Конфигурация приложения</param>
        public AuthenticationService(IConfiguration configuration) 
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Получить JWT токен
        /// </summary>
        /// <param name="user">Данные пользователя</param>
        /// <returns>JWT токен для дальнейшей авторизации</returns>
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
