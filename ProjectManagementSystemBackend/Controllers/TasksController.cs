using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Interfaces;
using ProjectManagementSystemBackend.Models.DTO;
using System.Net.WebSockets;
using System.Security.Claims;
using ProjectManagementSystemBackend.Common.CustomExceptions;
using IAuthorizationService = ProjectManagementSystemBackend.Interfaces.IAuthorizationService;

namespace ProjectManagementSystemBackend.Controllers
{
    /// <summary>
    /// Контроллер для управления задачами проекта
    /// </summary>
    /// <remarks>
    /// Позволяет создавать, получать, обновлять и удалять задачи
    /// Для работы требуется авторизация и соответствующие права доступа.
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TasksController : ControllerBase
    {
        ITaskService _taskService;
        IAuthorizationService _authorizationService;
        ITaskHistoryService _taskHistoryService;
        int? userId;
        int[] _userRoles = [1, 2, 3];
        int[] _adminRoles = [1, 2];
        int[] _ownerRoles = [1];
        int _userId => userId ??= Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

        /// <summary>
        /// Конструктор контроллера задач
        /// </summary>
        /// <param name="taskService">Сервис для работы с задачами</param>
        /// <param name="taskHistoryService">Сервис для работы с историей задач</param>
        /// <param name="authorizationService">Сервис авторизации</param>
        public TasksController(ITaskService taskService, ITaskHistoryService taskHistoryService, IAuthorizationService authorizationService) 
        {
            _taskService = taskService;
            _taskHistoryService = taskHistoryService;
            _authorizationService = authorizationService;
        }

        /// <summary>
        /// Получить задачи по коду статуса доски
        /// </summary>
        /// <param name="boardStatusId">ID статуса доски</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список задач</returns>
        /// <remarks>
        /// Для получения задач проект должен быть публичным или пользователь должен быть участником проекта
        /// 
        /// Пример запроса:
        /// GET /api/Tasks?boardStatusId=11
        /// </remarks>
        /// <response code="200">Список задач успешно получен</response>
        /// <response code="401">Недостаточно прав для просмотра</response>
        /// <response code="404">Задачи не найдены</response>
        [HttpGet]
        public async Task<IActionResult> GetAsync(int boardStatusId,CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByBoardStatusIdAsync(boardStatusId, _userId, _userRoles, cancellationToken);
            if(!isAuthorized)    
                return Unauthorized("You havent access to this action");

            var tasks = await _taskService.GetAsync(boardStatusId, cancellationToken);
            return tasks is null ? NotFound() : Ok(tasks);
        }

        /// <summary>
        /// Создать новую задачу
        /// </summary>
        /// <param name="task">Данные новой задачи</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Созданная задача</returns>
        /// <remarks>
        /// Для создания новой задачи необходимо иметь роль администратора и выше
        /// 
        /// Пример запроса:
        /// POST /api/Tasks
        /// {
        ///     "id": 0,
        ///     "name": "Task name",
        ///     "description": "Task description",
        ///     "priority": 2,
        ///     "lastUpdate": "2025-07-17T07:40:04.658Z",
        ///     "timeLimit": "2025-07-17T07:40:04.658Z",
        ///     "creatorId": 0,
        ///     "responsiblePersonId": 11,
        ///     "boardStatusId": 11
        /// }
        /// </remarks>
        /// <response code="200">Задача успешно создана</response>
        /// <response code="400">Некорректный ID ответственного</response>
        /// <response code="401">Недостаточно прав доступа</response>
        /// <response code="404">Участник не найден</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpPost]
        public async Task<IActionResult> PostAsync(TaskDTO task,CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByBoardStatusIdAsync(task.BoardStatusId, _userId, _adminRoles, cancellationToken);
            if(!isAuthorized)
                return Unauthorized("You havent access to this action");

            try
            {
                var newTask = await _taskService.PostAsync(task, _userId, cancellationToken);
                return Ok(newTask);
            }
            catch (BadRequestException ex) { return BadRequest(ex.Message); }
            catch (NotFoundException ex) { return NotFound(ex.Message); }
            catch (Exception) { return StatusCode(500, "Internal server error"); }
        }

        /// <summary>
        /// Обновить существующую задачу
        /// </summary>
        /// <param name="updatedTask">Обновленные данные задачи</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Статус операции</returns>
        /// <remarks>
        /// Для обновления задачи необходимо быть участником проекта с ролью администратора и выше
        /// 
        /// Пример запроса:
        /// PUT /api/Tasks
        /// {
        ///     "id": 11,
        ///     "name": "Task name",
        ///     "description": "Task description",
        ///     "priority": 2,
        ///     "lastUpdate": "2025-07-17T07:40:04.658Z",
        ///     "timeLimit": "2025-07-17T07:40:04.658Z",
        ///     "creatorId": 0,
        ///     "responsiblePersonId": 11,
        ///     "boardStatusId": 11
        /// }
        /// </remarks>
        /// <response code="204">Задача успешно обновлена</response>
        /// <response code="400">Некорректный статус доски или код ответственного</response>
        /// <response code="401">Недостаточно прав для обновления</response>
        /// <response code="404">Задача не найдена</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpPut]
        public async Task<IActionResult> UpdateAsync(TaskDTO updatedTask,CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByTaskIdAsync(updatedTask.Id, _userId, _adminRoles, cancellationToken);
            if(!isAuthorized)
                return Unauthorized("You havent access to this action");

            try
            {
                await _taskService.UpdateAsync(updatedTask, _userId, cancellationToken);
                return NoContent();
            }
            catch (NotFoundException ex) { return NotFound(ex.Message); }
            catch (BadRequestException ex) { return BadRequest(ex.Message); }
            catch (Exception) { return StatusCode(500, "Internal server error"); }
        }

        /// <summary>
        /// Удалить задачу
        /// </summary>
        /// <param name="taskId">ID задачи для удаления</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Статус операции</returns>
        /// <remarks>
        /// Для удаления задачи необходимо быть участником проекта и иметь роль администратора и выше
        /// 
        /// Пример запроса:
        /// DELETE /api/Tasks?taskId=11
        /// </remarks>
        /// <response code="204">Задача успешно удалена</response>
        /// <response code="401">Недостаточно прав для удаления</response>
        /// <response code="404">Задача не найдена</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(int taskId,CancellationToken cancellationToken)
        {
            bool isAuthorize = await _authorizationService.AccessByTaskIdAsync(taskId, _userId, _adminRoles, cancellationToken);
            if(!isAuthorize)
                return Unauthorized("You havent access to this action");

            try
            {
                await _taskService.DeleteAsync(taskId,cancellationToken);
                return NoContent();
            }
            catch (NotFoundException) { return NotFound(); }
            catch (Exception) { return StatusCode(500, "Internal server error"); }
        }

    }
}
