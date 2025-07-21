using Microsoft.AspNetCore.Authorization;
using ProjectManagementSystemBackend.Services.Authorization.Requirements.ProjectRequirements;

namespace ProjectManagementSystemBackend.Common
{
    /// <summary>
    /// Класс для настройки политик авторизации пользователей
    /// </summary>
    public class AuthorizationPoliciesConfig
    {
        /// <summary>
        /// Добавляет новые политики в сервис авторизации
        /// </summary>
        /// <param name="options">Настройки авторизации</param>
        public static void Configure(AuthorizationOptions options)
        {
            options.AddPolicy("ProjectOwnerPolicy", policy =>
            policy.Requirements.Add(new ProjectOwnerRequirement()));

            options.AddPolicy("ProjectAdminPolicy", policy =>
            policy.Requirements.Add(new ProjectAdminRequirement()));
        }
    }
}
