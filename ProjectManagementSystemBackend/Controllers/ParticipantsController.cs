using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Interfaces;
using ProjectManagementSystemBackend.Models;
using ProjectManagementSystemBackend.Models.DTO;
using System;
using System.Security.Claims;
using IAuthorizationService = ProjectManagementSystemBackend.Interfaces.IAuthorizationService;

namespace ProjectManagementSystemBackend.Controllers
{
    /// <summary>
    /// Контроллер для управления участниками проектов
    /// </summary>
    /// <remarks>
    /// Позволяет добавлять, просматривать, изменять и удалять участников проектов.
    /// Требует авторизации и соответствующих прав доступа.
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ParticipantsController : ControllerBase
    {
        ApplicationContext _context;
        IAuthorizationService _authorizationService;
        IParticipantService _participantService;

        int? userId;
        int _userId => userId ??= Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        int[] _userRoles = [1, 2, 3];
        int[] _adminRoles = [1, 2];
        int[] _ownerRoles = [1];

        /// <summary>
        /// Конструктор контроллера участников
        /// </summary>
        /// <param name="participantService">Сервис работы с участниками</param>
        /// <param name="authorizationService">Сервис авторизации</param>
        public ParticipantsController(IParticipantService participantService, IAuthorizationService authorizationService) 
        {
            _participantService = participantService;
            _authorizationService = authorizationService;
        }

        /// <summary>
        /// Получить список участников проекта
        /// </summary>
        /// <param name="projectId">ID проекта</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список участников проекта</returns>
        /// <remarks>
        /// Для получения списка участников проект должен быть публичным или необходимо быть участником проекта
        /// 
        /// Пример запроса:
        /// GET /api/Participants?projectId=123
        /// </remarks>
        /// <response code="200">Список участников успешно получен</response>
        /// <response code="401">Недостаточно прав доступа</response>
        /// <response code="404">Участники не найдены</response>
        [HttpGet]
        public async Task<IActionResult> GetAsync(int projectId, CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByProjectIdAsync(projectId, _userId, _userRoles, cancellationToken);
            if (!isAuthorized)
                return Unauthorized("You havent access to this action");

            var participants = await _participantService.GetAsync(projectId,cancellationToken);
            return participants is null ? NotFound() : Ok(participants);
        }

        /// <summary>
        /// Добавить нового участника в проект
        /// </summary>
        /// <param name="participant">Данные участника</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>ID созданного участника</returns>
        /// <remarks>
        /// Для добавления нового участника необходимо иметь права владельца проекта
        /// 
        /// Пример запроса:
        /// POST /api/Participants
        /// {
        ///     "id": 0,
        ///     "projectId": 11,
        ///     "userId": 11,
        ///     "roleId": 3
        /// }
        /// 
        /// </remarks>
        /// <response code="200">Участник успешно добавлен</response>
        /// <response code="400">Некорректные данные</response>
        /// <response code="401">Недостаточно прав для добавления</response>
        /// <response code="409">Конфликт данных (пользователь уже является участником проекта)</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpPost]
        public async Task<IActionResult> PostAsync(ParticipantDTO participant, CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByProjectIdAsync(participant.ProjectId, _userId, _ownerRoles, cancellationToken);
            if (!isAuthorized)
                return Unauthorized("You havent access to this action");
            try
            {
                var newParticipant = await _participantService.PostAsync(participant, cancellationToken);
                return Ok(newParticipant.Id);
            }
            catch (InvalidDataException ex) { return BadRequest(ex.Message); }
            catch (InvalidOperationException ex) { return Conflict(ex.Message); }
            catch (Exception) { return StatusCode(500, "Internal server error"); }
        }

        /// <summary>
        /// Обновить данные участника
        /// </summary>
        /// <param name="newParticipant">Новые данные участника</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Статус операции</returns>
        /// <remarks>
        /// Для обновления данных об участнике необходимо иметь роль владельца проекта
        /// 
        /// Пример запроса:
        /// PUT /api/Participants
        /// {
        ///     "id": 11,
        ///     "projectId": 11,
        ///     "userId": 11,
        ///     "roleId": 3
        /// }
        /// </remarks>
        /// <response code="204">Данные участника обновлены</response>
        /// <response code="400">Некорректные данные</response>
        /// <response code="401">Недостаточно прав для изменения</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpPut]
        public async Task<IActionResult> UpdateAsync(ParticipantDTO newParticipant,CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByParticipantIdAsync(newParticipant.Id, _userId, _ownerRoles, cancellationToken);
            if (!isAuthorized)
                return Unauthorized("You havent access to this action");

            try
            {
                await _participantService.UpdateAsync(newParticipant, cancellationToken);
                return NoContent();
            }
            catch (InvalidDataException ex) { return BadRequest(ex.Message); }
            catch (Exception) { return StatusCode(500, "Internal server error"); }
        }

        /// <summary>
        /// Удалить участника из проекта
        /// </summary>
        /// <param name="participantId">ID участника</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Статус операции</returns>
        /// <remarks>
        /// Для удаления участника проекта необходимо иметь роль владельца проекта
        /// 
        /// Пример запроса:
        /// DELETE /api/Participants?participantId=11
        /// </remarks>
        /// <response code="204">Участник успешно удален</response>
        /// <response code="401">Недостаточно прав для удаления</response>
        /// <response code="404">Участник не найден</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(int participantId, CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByParticipantIdAsync(participantId, _userId, _ownerRoles, cancellationToken);
            if (!isAuthorized)
                return Unauthorized("You havent access to this action");

            try
            {
                await _participantService.DeleteAsync(participantId, cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException) { return NotFound(); }
            catch (Exception) { return StatusCode(500, "Internal server error"); }
        }
    }
}
