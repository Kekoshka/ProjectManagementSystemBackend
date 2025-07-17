using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Interfaces;
using ProjectManagementSystemBackend.Models;
using ProjectManagementSystemBackend.Models.DTO;
using ProjectManagementSystemBackend.Services;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using IAuthorizationService = ProjectManagementSystemBackend.Interfaces.IAuthorizationService;

namespace ProjectManagementSystemBackend.Controllers
{
    /// <summary>
    /// Контроллер для управления комментариями к задачам
    /// </summary>
    /// <remarks>
    /// Позволяет создавать, просматривать, обновлять и удалять комментарии.
    /// Для работы требуется авторизация.
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CommentsController : ControllerBase
    {
        ApplicationContext _context;
        IAuthorizationService _authorizationService;
        ICommentService _commentService;
        
        int? userId;
        int _userId => userId ??= Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

        int[] _userRoles = [1, 2, 3];
        int[] _adminRoles = [1, 2];
        int[] _ownerRoles = [1];
        /// <summary>
        /// Конструктор контроллера комментариев
        /// </summary>
        /// <param name="commentService">Сервис для работы с комментариями</param>
        /// <param name="authorizationService">Сервис авторизации</param>
        public CommentsController(ICommentService commentService, IAuthorizationService authorizationService) 
        {
            _commentService = commentService;
            _authorizationService = authorizationService;
        }
        /// <summary>
        /// Получить все комментарии для задачи
        /// </summary>
        /// <param name="taskId">ID задачи</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список комментариев</returns>
        /// <remarks>
        /// Для получения списка комментариев необходимо быть участником проекта и иметь роль пользователя и выше
        /// 
        /// Пример запроса:
        /// GET /api/Comments?taskId=11
        /// </remarks>
        /// <response code="200">Комментарии успешно получены</response>
        /// <response code="401">Нет доступа к задаче</response>
        /// <response code="404">Комментарии не найдены</response> 
        [HttpGet]
        public async Task<IActionResult> GetAsync(int taskId, CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByTaskIdAsync(taskId, _userId, _userRoles, cancellationToken);
            if(!isAuthorized)
                return Unauthorized("You havent access to this action");

            var comments = await _commentService.GetAsync(taskId, cancellationToken);
            return comments is null ? NotFound() : Ok(comments);
        }
        /// <summary>
        /// Создать новый комментарий
        /// </summary>
        /// <param name="comment">DTO комментария</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>ID созданного комментария</returns>
        /// <remarks>
        /// Для публикации комментария необходимо быть участником проекта с ролью пользователя и выше
        /// 
        /// Пример запроса:
        /// POST /api/Comments
        /// {
        ///     "id": 0,
        ///     "message": "new message",
        ///     "participantId": 0,
        ///     "taskId": 11
        /// }
        /// </remarks>
        /// <response code="200">Комментарий успешно создан</response>
        /// <response code="400">Некорректные данные</response>
        /// <response code="401">Нет прав на создание</response> 
            [HttpPost]
        public async Task<IActionResult> PostAsync(CommentDTO comment, CancellationToken cancellationToken)
        {
            bool participantId = await _authorizationService.AccessByTaskIdAsync(comment.TaskId, _userId, _userRoles, cancellationToken);
            if (participantId is false)
                return Unauthorized("You havent access to this action");

            var newComment = await _commentService.PostAsync(comment, _userId, cancellationToken);
            return Ok(newComment.Id);
        }
        /// <summary>
        /// Обновить существующий комментарий
        /// </summary>
        /// <param name="comment">DTO комментария с обновленными данными</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Статус операции</returns>
        /// <remarks>
        /// Для обновления комментария необходимо быть создателем комментария 
        /// 
        /// Пример запроса:
        /// UPDATE /api/Comments
        /// {
        ///     "id": 11,
        ///     "message": "new updated message",
        ///     "participantId": 0,
        ///     "taskId": 11
        /// }
        /// </remarks>
        /// <response code="204">Комментарий обновлен</response>
        /// <response code="401">Нет прав на редактирование</response>
        /// <response code="404">Комментарий не найден</response>
        [HttpPut]
        public async Task<IActionResult> UpdateAsync(CommentDTO comment,CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByCommentIdAsync(comment.Id, _userId, cancellationToken);
            if(!isAuthorized)
                return Unauthorized("You havent access to this action");

            var newComment = await _commentService.UpdateAsync(comment, cancellationToken);
            return newComment is null ? NotFound() : NoContent();
        }
        /// <summary>
        /// Удалить комментарий
        /// </summary>
        /// <param name="commentId">ID комментария</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Статус операции</returns>
        /// <remarks>
        /// Для удаления комментария необходимо быть создателем комментария
        /// 
        /// Пример запроса:
        /// DELETE /api/Comments?commentId=1
        /// </remarks>
        /// <response code="204">Комментарий удален</response>
        /// <response code="401">Нет прав на удаление</response>
        /// <response code="404">Комментарий не найден</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(int commentId,CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByCommentIdAsync(commentId, _userId, cancellationToken);
            if (!isAuthorized)
                return Unauthorized("You havent access to this action");

            try
            {
                await _commentService.DeleteAsync(commentId, cancellationToken);
                return NoContent();
            }
            catch(KeyNotFoundException) { return NotFound(); }
            catch(Exception) { return StatusCode(500, "Internal server error"); }
        }
    }
}
