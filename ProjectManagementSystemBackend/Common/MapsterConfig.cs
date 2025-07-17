using Mapster;
using ProjectManagementSystemBackend.Models;
using ProjectManagementSystemBackend.Models.DTO;

namespace ProjectManagementSystemBackend.Common
{
    /// <summary>
    /// Класс, хранящий конфигурацию для Mapster
    /// </summary>
    public class MapsterConfig : IRegister
    {
        /// <summary>
        /// Задает конфигурацию для Mapster
        /// </summary>
        /// <param name="config">Параметры конфигурации</param>
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<BoardStatus, StatusDTO>()
                .Map(dest => dest.Name, src => src.Status == null ? null : src.Status.Name);

            config.NewConfig<TaskHistory, TaskHistoryDTO>()
                .Map(dest => dest.ActionTypeName, src => src.ActionType == null ? null : src.ActionType.Name)
                .Map(dest => dest.UserName, src => src.User == null ? null : src.User.Name);
        }
    }
}
