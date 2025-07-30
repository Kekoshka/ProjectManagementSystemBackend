using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Interfaces;
using ProjectManagementSystemBackend.Models;
using ProjectManagementSystemBackend.Models.DTO;
using System.ComponentModel;
using System.Security.Claims;
using ProjectManagementSystemBackend.Common.CustomExceptions;
using IAuthorizationService = ProjectManagementSystemBackend.Interfaces.IAuthorizationService;

namespace ProjectManagementSystemBackend.Controllers
{
    /// <summary>
    /// Контроллер для управления статусами задач
    /// </summary>
    /// <remarks>
    /// Позволяет работать со статусами задач
    /// Требует авторизации и соответствующих прав доступа.
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StatusesController : ControllerBase
    {
        IStatusService _statusService;
        IAuthorizationService _authorizationService;

        int? userId;
        int _userId => userId ??= Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        int[] _userRoles = [1, 2, 3];
        int[] _adminRoles = [1, 2];

        /// <summary>
        /// Конструктор контроллера статусов
        /// </summary>
        /// <param name="statusService">Сервис работы со статусами</param>
        /// <param name="authorizationService">Сервис авторизации</param>
        public StatusesController(IStatusService statusService, IAuthorizationService authorizationService) 
        {
            _statusService = statusService;
            _authorizationService = authorizationService;
        }

        /// <summary>
        /// Получить статусы для указанной доски
        /// </summary>
        /// <param name="baseBoardId">ID базовой доски</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Список статусов доски</returns>
        /// <remarks>
        /// Для получения статусов доски проект должен быть публичным или необходимо быть участником проекта
        /// 
        /// Пример запроса:
        /// GET /api/Statuses?baseBoardId=11
        /// </remarks>
        /// <response code="200">Список статусов успешно получен</response>
        /// <response code="401">Недостаточно прав доступа</response>
        /// <response code="404">Статусы не найдены</response>
        [HttpGet]
        public async Task<IActionResult> GetAsync(int baseBoardId, CancellationToken cancellationToken)
        {
            bool isAuthorize = await _authorizationService.AccessByBoardIdAsync(baseBoardId, _userId, _userRoles, cancellationToken);
            if (!isAuthorize)
                return Unauthorized("You havent access to this action");

            var statuses = await _statusService.GetAsync(baseBoardId, cancellationToken);
            return statuses is null ? NotFound() : Ok(statuses);
        }

        /// <summary>
        /// Создать новый статус
        /// </summary>
        /// <param name="status">Данные нового статуса</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Созданный статус</returns>
        /// <remarks>
        /// Для создания нового статуса необходимо иметь роль администратора и выше
        /// 
        /// Пример запроса:
        /// POST /api/Statuses
        /// {
        ///     "id": 0,
        ///     "name": "Status name",
        ///     "baseBoardId": 11,
        ///     "statusId": 0
        /// }
        /// </remarks>
        /// <response code="200">Статус успешно создан</response>
        /// <response code="401">Недостаточно прав доступа</response>
        /// <response code="404">Базовая доска не найдена</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpPost]
        public async Task<IActionResult> PostAsync(StatusDTO status, CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByBoardIdAsync(status.BaseBoardId, _userId, _adminRoles, cancellationToken);
            if (!isAuthorized)
                return Unauthorized("You havent access to this action");

            var newStatus = await _statusService.PostAsync(status, cancellationToken);
            return Ok(newStatus);
        }

        /// <summary>
        /// Обновить существующий статус
        /// </summary>
        /// <param name="status">Обновленные данные статуса</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Статус операции</returns>
        /// <remarks>
        /// Для обновления статуса необходимо иметь роль администратора и выше
        /// 
        /// Пример запроса:
        /// PUT /api/Statuses
        /// {
        ///     "id": 11,
        ///     "name": "New status name",
        ///     "baseBoardId": 11,
        ///     "statusId": 0
        /// }
        /// </remarks>
        /// <response code="204">Статус успешно обновлен</response>
        /// <response code="401">Недостаточно прав для обновления</response>
        /// <response code="404">Статус не найден</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpPut]
        public async Task<IActionResult> UpdateAsync(StatusDTO status, CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByBoardStatusIdAsync(status.Id, _userId, _adminRoles, cancellationToken);
            if(!isAuthorized)
                return Unauthorized("You havent access to this action");

            await _statusService.UpdateAsync(status, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Удалить статус
        /// </summary>
        /// <param name="boardStatusId">ID статуса доски для удаления</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Статус операции</returns>
        /// <remarks>
        /// Для удаления статуса необходимо иметь роль администратора и выше
        /// 
        /// Пример запроса:
        /// DELETE /api/Statuses?boardStatusId=11
        /// </remarks>
        /// <response code="204">Статус успешно удален</response>
        /// <response code="401">Недостаточно прав для удаления</response>
        /// <response code="404">Статус не найден</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(int boardStatusId, CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByBoardStatusIdAsync(boardStatusId, _userId, _adminRoles, cancellationToken);
            if(!isAuthorized)    
                return Unauthorized("You havent access to this action");

            await _statusService.DeleteAsync(boardStatusId, cancellationToken);
            return NoContent();
        }
    }
}
