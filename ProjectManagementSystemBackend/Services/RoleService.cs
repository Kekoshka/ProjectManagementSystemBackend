using Mapster;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Interfaces;
using ProjectManagementSystemBackend.Common.CustomExceptions;
using ProjectManagementSystemBackend.Models.DTO;

namespace ProjectManagementSystemBackend.Services
{
    /// <summary>
    /// Сервис для управления ролями
    /// </summary>
    /// <remarks>
    /// Позволяет получить все роли системы
    /// </remarks>
    public class RoleService : IRoleService
    {
        ApplicationContext _context;
        /// <summary>
        /// Конструктор сервиса управления ролями
        /// </summary>
        /// <param name="context">Контекст базы данных</param>
        public RoleService(ApplicationContext context) 
        {
            _context = context;
        }

        /// <summary>
        /// Получить все доступные роли системы
        /// </summary>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Список DTO ролей</returns>
        public async Task<IEnumerable<RoleDTO>> GetAsync(CancellationToken cancellationToken)
        {
            var roles = await _context.Roles
                .AsNoTracking()
                .ProjectToType<RoleDTO>()
                .ToListAsync(cancellationToken);
            return roles;
        }
    }
}
