using FluentValidation;
using ProjectManagementSystemBackend.Services;
using System.Reflection;

namespace ProjectManagementSystemBackend.Common
{
    /// <summary>
    /// Класс для регистрации сервисов в системе
    /// </summary>
    public static class RegisterServices
    {
        /// <summary>
        /// Регистрирует сервисы в текущей сборке
        /// </summary>
        /// <param name="services">Коллекция сервисов</param>
        /// <remarks>
        /// Регистрирует все сервисы в текущей сборке, названия которых заканчиваются на "Service",
        /// а также которые имеют интерфейс с соответствующим названием и префиксом "I"
        /// </remarks>
        public static void RegisterExecutingAsseblyServices(IServiceCollection services)
        {
            var serviceTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(st => st.IsClass && !st.IsAbstract && st.Name.EndsWith("Service"));
            foreach (var serviceType in serviceTypes)
            {
                var interfaceType = serviceType.GetInterfaces()
                    .FirstOrDefault(it => it.Name == $"I{serviceType.Name}");
                if (interfaceType is not null)
                    services.AddScoped(interfaceType, serviceType);
            }

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        }    
    }
}
