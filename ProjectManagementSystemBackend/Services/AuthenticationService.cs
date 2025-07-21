using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Interfaces;
using ProjectManagementSystemBackend.Models;
using ProjectManagementSystemBackend.Models.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static Azure.Core.HttpHeader;

namespace ProjectManagementSystemBackend.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        JWTOptions _jwtOptions;
        
        /// <summary>
        /// Конструктор сервиса аутентификации
        /// </summary>
        /// <param name="configuration">Конфигурация приложения</param>
        public AuthenticationService(IOptions<JWTOptions> jwtOptions) 
        {
            _jwtOptions = jwtOptions.Value;
        }

        /// <summary>
        /// Получить JWT токен
        /// </summary>
        /// <param name="user">Данные пользователя</param>
        /// <returns>JWT токен для дальнейшей авторизации</returns>
        /// <remarks>
        /// JWT токен выдается на 60 минут
        /// В токене хранятся данные о ID и имени пользователя, 
        /// </remarks>
        public string GetJWT(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
            };

            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: user.Id.ToString(),
                expires: DateTime.UtcNow.AddMinutes(_jwtOptions.LifeTimeFromMinutes),
                claims: claims,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key)), SecurityAlgorithms.HmacSha256));
           
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}
