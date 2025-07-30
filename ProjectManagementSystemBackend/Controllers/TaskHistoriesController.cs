using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Interfaces;
using System.Security.Claims;
using ProjectManagementSystemBackend.Common.CustomExceptions;
using IAuthorizationService = ProjectManagementSystemBackend.Interfaces.IAuthorizationService;

namespace ProjectManagementSystemBackend.Controllers
{
    /// <summary>
    /// Контроллер для просмотра истории задач
    /// </summary>
    /// <remarks>
    /// Позволяет получать данные о истории задачи
    /// Требует авторизации и соответствующих прав доступа
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TaskHistoriesController : ControllerBase
    {
        IAuthorizationService _authorizationService;
        ITaskHistoryService _taskHistoryService;

        int? userId;
        int[] _userRoles = [1, 2, 3];
        int _userId => userId ??= Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

        /// <summary>
        /// Конструктор контроллера истории задач
        /// </summary>
        /// <param name="taskHistoryService">Сервис для работы с историей задач</param>
        /// <param name="authorizationService">Сервис авторизации</param>
        public TaskHistoriesController(ITaskHistoryService taskHistoryService, IAuthorizationService authorizationService) 
        {
            _taskHistoryService = taskHistoryService;
            _authorizationService = authorizationService;
        }

        /// <summary>
        /// Получить историю для указанной задачи
        /// </summary>
        /// <param name="taskId">ID задачи</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Список истории измененений задачи</returns>
        /// <remarks>
        /// Возвращает хронологический список всех изменений задачи.
        /// Для получения истории задачи проект должен быть публичным или пользователь должен быть участником проекта
        /// 
        /// Пример запроса:
        /// GET /api/Taskhistories?taskId=11
        /// </remarks>
        /// <response code="200">История изменений успешно получена</response>
        /// <response code="401">Недостаточно прав для просмотра</response>
        /// <response code="404">История изменений не найдена</response>
        [HttpGet]
        public async Task<IActionResult> GetAsync(int taskId, CancellationToken cancellationToken)
        {
            bool isAuthorize = await _authorizationService.AccessByTaskIdAsync(taskId, _userId, _userRoles, cancellationToken);
            if(!isAuthorize)
                return Unauthorized("You havent access to this action");
            var taskHistories = await _taskHistoryService.GetAsync(taskId, cancellationToken);
            return taskHistories is null ? NotFound() : Ok(taskHistories);
        }   
    }
}
