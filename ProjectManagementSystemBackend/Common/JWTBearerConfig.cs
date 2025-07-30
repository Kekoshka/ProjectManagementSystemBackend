using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Models.Options;
using System.Text;

namespace ProjectManagementSystemBackend.Common
{
    /// <summary>
    /// Класс для настройки авторизации по JWT
    /// </summary>
    public static class JWTBearerConfig
    {
        /// <summary>
        /// Конфигурация для авторизации по JWT
        /// </summary>
        /// <param name="services">Коллекция сервисов приложения</param>
        /// <param name="configuration">Конфигурация</param>
        public static void ConfigureJWTAuthentication(IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            JWTOptions JWToptions = serviceProvider.GetService<IOptions<JWTOptions>>().Value;

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                options.TokenValidationParameters = new TokenValidationParameters
                {

                    ValidateIssuer = true,
                    ValidIssuer = JWToptions.Issuer,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWToptions.Key)),
                    AudienceValidator = (audiences, securityToken, validationParameters) =>
                        ValidateAudience(services, audiences)
                });
        }
        private static bool ValidateAudience(IServiceCollection services, IEnumerable<string> audiences)
        {
            using var scope = services.BuildServiceProvider().CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            return dbContext.Users.Any(u => u.Id.ToString() == audiences.FirstOrDefault());
        }
    }
}
