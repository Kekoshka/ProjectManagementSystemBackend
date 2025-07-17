using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Interfaces;
using ProjectManagementSystemBackend.Models.DTO;

namespace ProjectManagementSystemBackend.Controllers
{
    /// <summary>
    /// Контроллер для управления ролями пользователей
    /// </summary>
    /// <remarks>
    /// Позволяет просматривать возможные роли пользователей
    /// Для доступа требуется авторизацияы
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RolesController : ControllerBase
    {
        IRoleService _roleService;
        /// <summary>
        /// Конструктор контроллера ролей
        /// </summary>
        /// <param name="roleService">Сервис для работы с ролями</param>
        public RolesController(IRoleService roleService) 
        {
            _roleService = roleService;
        }

        /// <summary>
        /// Получить список всех доступных ролей
        /// </summary>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Список ролей системы</returns>
        /// <remarks>
        /// Доступен любому авторизованому пользователю системы
        /// 
        /// Пример запроса:
        /// GET /api/Roles
        /// </remarks>
        /// <response code="200">Список ролей успешно получен</response>
        /// <response code="401">Требуется авторизация</response>
        /// <response code="404">Роли не найдены</response>
        [HttpGet]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
        {
            var roles = _roleService.GetAsync(cancellationToken);

            return roles is null ? NotFound() : Ok(roles);
        }
    }
}
