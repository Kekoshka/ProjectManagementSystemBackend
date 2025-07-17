using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Interfaces;
using ProjectManagementSystemBackend.Models;
using ProjectManagementSystemBackend.Models.DTO;
using System.Security.Claims;

namespace ProjectManagementSystemBackend.Controllers
{
    /// <summary>
    /// Контроллер для управления досками проектов (Kanban и Scrum)
    /// </summary>
    /// <remarks>
    /// Позволяет создавать, получать, обновлять и удалять доски проектов.
    /// Поддерживает два типа досок: Kanban и Scrum.
    /// Для всех операций требуется авторизация и соответствующие права доступа.
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BoardsController : ControllerBase
    {
        IBoardService _boardService;
        Interfaces.IAuthorizationService _authorizationService;

        int? userId;
        int _userId => userId ??= Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        int[] _userRoles = [1, 2, 3];
        int[] _adminRoles = [1, 2];
        int[] _ownerRoles = [1];

        /// <summary>
        /// Конструктор контроллера досок
        /// </summary>
        /// <param name="boardService">Сервис для работы с досками</param>
        /// <param name="authorizationService">Сервис авторизации</param>
        public BoardsController(IBoardService boardService, Interfaces.IAuthorizationService authorizationService)
        {
            _boardService = boardService;
            _authorizationService = authorizationService;
        }

        /// <summary>
        /// Получить все базовые доски проекта
        /// </summary>
        /// <param name="projectId">ID проекта</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список базовых досок проекта</returns>
        /// <remarks>
        /// Для получения базовых досок проект должен быть открытым или пользователь должен быть участником проекта
        /// 
        /// Пример запроса:
        /// GET /api/Boards/getBaseBoardsByProjectId?projectId=11
        /// </remarks>
        /// <response code="200">Возвращает список базовых досок проекта</response>
        /// <response code="401">Недостаточно прав для доступа к проекту</response>
        /// <response code="404">Доски для указанного проекта не найдены</response>
        [HttpGet("getBaseBoardsByProjectId")]
        public async Task<IActionResult> GetBoardsByProjectIdAsync(int projectId, CancellationToken cancellationToken)
        {
            var IsAuthorize = await _authorizationService.AccessByProjectIdAsync(projectId, _userId, _userRoles, cancellationToken);
            if (!IsAuthorize)
                return Unauthorized("You havent access to this action");

            var boards = await _boardService.GetBoardsByProjectIdAsync(projectId, cancellationToken);

            return boards.Count() == 0 || boards is null ? NotFound() : Ok(boards);
        }

        /// <summary>
        /// Получить доску по ID базовой доски
        /// </summary>
        /// <param name="baseBoardId">ID базовой доски</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Данные доски (KanbanBoardDTO или ScrumBoardDTO)</returns>
        /// <remarks>
        /// Для получения доски проект должен быть открытым или пользователь должен быть участником проекта
        /// 
        /// Пример запроса:
        /// GET /api/Boards/getBoardByBaseBoardId?baseBoardId=11
        /// </remarks>
        /// <response code="200">Возвращает данные доски (Kanban или Scrum)</response>
        /// <response code="401">Недостаточно прав доступа</response>
        /// <response code="404">Доска с указанным ID не найдена</response>
        [HttpGet("getBoardByBaseBoardId")]
        public async Task<IActionResult> GetByBaseBoardIdAsync(int baseBoardId, CancellationToken cancellationToken)
        {
            bool IsAuthorize = await _authorizationService.AccessByBoardIdAsync(baseBoardId, _userId, _userRoles, cancellationToken);
            if (!IsAuthorize)
                return Unauthorized("You havent access to this action");

            object? abstractBoard = await _boardService.GetByBaseBoardIdAsync(baseBoardId, cancellationToken);
            if (abstractBoard is KanbanBoardDTO cbDTO)
                return Ok(cbDTO);
            if (abstractBoard is ScrumBoardDTO sbDTO)
                return Ok(sbDTO);
            return NotFound();
            
        }

        /// <summary>
        /// Создать новую Kanban доску
        /// </summary>
        /// <param name="kanbanBoard">Данные для создания доски</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Созданная Kanban доска</returns>
        /// <remarks>
        /// Для создания доски пользователь должен быть участником проекта и иметь роль администратора и выше
        /// 
        /// Пример запроса:
        /// POST /api/boards/postKanbanBoard
        /// {
        ///     "id": 0,
        ///     "taskLimit": 11,
        ///     "baseBoardId": 11,
        ///     "baseBoard": 
        ///     {
        ///         "id": 11,
        ///         "name": "Name base board",
        ///         "description": "Description base board",
        ///         "projectId": 11
        ///     }
        ///}
        /// </remarks>
        /// <response code="200">Возвращает созданную Kanban доску</response>
        /// <response code="401">Недостаточно прав для создания доски</response>
        [HttpPost("postKanbanBoard")]
        public async Task<IActionResult> PostKanbanAsync(KanbanBoardDTO kanbanBoard, CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByBoardIdAsync(kanbanBoard.BaseBoard.Id, _userId, _adminRoles, cancellationToken);
            if (!isAuthorized)
                return Unauthorized("You havent access to this action");

            var newKanbanBoard = await _boardService.PostKanbanAsync(kanbanBoard, cancellationToken);
            return Ok(newKanbanBoard);
        }

        /// <summary>
        /// Создать новую Scrum доску
        /// </summary>
        /// <param name="scrumBoard">Данные для создания доски</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Созданная Scrum доска</returns>
        /// <remarks>
        /// Для создания доски пользователь должен быть участником проекта и иметь роль администратора и выше
        /// 
        /// Пример запроса:
        /// POST /api/Boards/postScrumBoard
        /// {
        ///     "id": 0,
        ///     "timeLimit": "2025-07-17T08:07:50.645Z",
        ///     "baseBoardId": 11,
        ///     "baseBoard": 
        ///     {
        ///         "id": 11,
        ///         "name": "Name base board",
        ///         "description": "Description base board",
        ///         "projectId": 11
        ///     }
        ///}
        /// </remarks>
        /// <response code="200">Возвращает созданную Scrum доску</response>
        /// <response code="401">Недостаточно прав для создания доски</response>
        /// <response code="401">Недостаточно прав доступа</response>
        [HttpPost("postScrumBoard")]
        public async Task<IActionResult> PostScrumAsync(ScrumBoardDTO scrumBoard, CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByProjectIdAsync(scrumBoard.BaseBoard.ProjectId, _userId, _adminRoles, cancellationToken);
            if (!isAuthorized)
                return Unauthorized("You havent access to this action");

            var newScrumBoard = await _boardService.PostScrumAsync(scrumBoard, cancellationToken);
            return Ok(newScrumBoard);
        }

        /// <summary>
        /// Обновить Kanban доску
        /// </summary>
        /// <param name="newKanbanBoard">Новые данные доски</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Статус операции</returns>
        /// <remarks>
        /// 
        /// 
        /// Пример запроса:
        /// PUT /api/Boards/updateKanbanBoard
        /// {
        ///     "id": 11,
        ///     "taskLimit": 11,
        ///     "baseBoardId": 11,
        ///     "baseBoard": 
        ///     {
        ///         "id": 11,
        ///         "name": "Name base board update",
        ///         "description": "Description base board update",
        ///         "projectId": 11
        ///     }
        ///}
        /// </remarks>
        /// <response code="204">Доска успешно обновлена</response>
        /// <response code="401">Недостаточно прав для обновления</response>
        /// <response code="404">Доска не найдена</response>
        /// <response code="422">Несоответствие данных (например, неверный projectId)</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPut("updateKanbanBoard")]
        public async Task<IActionResult> UpdateKanbanBoardAsync(KanbanBoardDTO newKanbanBoard, CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByBoardIdAsync(newKanbanBoard.BaseBoard.Id, _userId, _adminRoles, cancellationToken);
            if (!isAuthorized)
                return Unauthorized("You havent access to this action");

            try
            {
                await _boardService.UpdateKanbanBoardAsync(newKanbanBoard, cancellationToken);   
                return NoContent();
            }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
            catch(InvalidOperationException ex) { return UnprocessableEntity(ex.Message); }
            catch(Exception) { return StatusCode(500, "InternalServerError"); }
        }

        /// <summary>
        /// Обновить Scrum доску
        /// </summary>
        /// <param name="newScrumBoard">Новые данные доски</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Статус операции</returns>
        /// <remarks>
        /// Пример запроса:
        /// PUT /api/Boards/updateScrumBoard
        /// {
        ///     "id": 11,
        ///     "timeLimit": "2025-07-17T08:07:50.645Z",
        ///     "baseBoardId": 11,
        ///     "baseBoard": 
        ///     {
        ///         "id": 11,
        ///         "name": "Name base board update",
        ///         "description": "Description base board update",
        ///         "projectId": 11
        ///     }
        ///}
        /// </remarks>
        /// <response code="204">Доска успешно обновлена</response>
        /// <response code="401">Недостаточно прав для обновления</response>
        /// <response code="404">Доска не найдена</response>
        /// <response code="422">Несоответствие данных (например, неверный projectId)</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPut("updateScrumBoard")]
        public async Task<IActionResult> UpdateScrumBoardAsync(ScrumBoardDTO newScrumBoard, CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByProjectIdAsync(newScrumBoard.BaseBoard.ProjectId, _userId, _adminRoles, cancellationToken);
            if (!isAuthorized)
                return Unauthorized("You havent access to this action");

            try
            {
                await _boardService.UpdateScrumBoardAsync(newScrumBoard, cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
            catch (InvalidOperationException ex) { return UnprocessableEntity(ex.Message); }
            catch (Exception) { return StatusCode(500, "InternalServerError"); }
        }

        /// <summary>
        /// Удалить доску
        /// </summary>
        /// <param name="baseBoardId">ID базовой доски</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Статус операции</returns>
        /// <remarks>
        /// Пример запроса:
        /// DELETE /api/Boards?baseBoardId=11
        /// </remarks>
        /// <response code="204">Доска успешно удалена</response>
        /// <response code="401">Недостаточно прав для удаления</response>
        /// <response code="404">Доска не найдена</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(int baseBoardId, CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByBoardIdAsync(baseBoardId, _userId, _ownerRoles, cancellationToken);
            if (!isAuthorized)
                return Unauthorized("You havent access to this action");

            try
            {
                await _boardService.DeleteAsync(baseBoardId, cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
            catch (Exception) { return StatusCode(500, "InternalServerError"); }
        }
    }
}
