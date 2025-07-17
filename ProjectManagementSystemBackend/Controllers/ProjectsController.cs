using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Identity.Client;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Interfaces;
using ProjectManagementSystemBackend.Models;
using ProjectManagementSystemBackend.Models.DTO;
using System.Security.Claims;
using IAuthorizationService = ProjectManagementSystemBackend.Interfaces.IAuthorizationService;

namespace ProjectManagementSystemBackend.Controllers
{
    /// <summary>
    /// Контроллер для управления проектами
    /// </summary>
    /// <remarks>
    /// Позволяет создавать, просматривать, обновлять и удалять проекты
    /// Требует авторизации
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        IAuthorizationService _authorizationService;
        IProjectService _projectService;

        int? userId;
        int _userId => userId ??= Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        int[] _userRoles = [1, 2, 3];
        int[] _adminRoles = [1, 2];
        int[] _ownerRoles = [1];
        /// <summary>
        /// Конструктор контроллера проектов
        /// </summary>
        /// <param name="projectService">Сервис для работы с проектами</param>
        /// <param name="authorizationService">Сервис авторизации</param>
        public ProjectsController(IProjectService projectService, IAuthorizationService authorizationService) 
        {
            _projectService = projectService;
            _authorizationService = authorizationService;
        }

        /// <summary>
        /// Получить список доступных проектов
        /// </summary>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Список проектов</returns>
        /// <remarks>
        /// Возвращает все проекты, доступные текущему пользователю
        /// 
        /// Пример запроса:
        /// GET /api/Projects
        /// </remarks>
        /// <response code="200">Список проектов успешно получен</response>
        /// <response code="404">Проекты не найдены</response>
        [HttpGet]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
        {
            var projects = await _projectService.GetAsync(_userId, cancellationToken);
            return projects is null ? NotFound() : Ok(projects);
        }

        /// <summary>
        /// Создать новый проект
        /// </summary>
        /// <param name="project">Данные нового проекта</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>ID созданного проекта</returns>
        /// <remarks>
        /// Создатель проекта автоматически становится участником и получает роль владельца
        /// 
        /// Пример запроса:
        /// POST /api/Projects
        /// {
        ///     "id": 0,
        ///     "name": "project name",
        ///     "description": "project description",
        ///     "security": true
        /// }
        /// </remarks>
        /// <response code="200">Проект успешно создан</response>
        /// <response code="400">Некорректные данные</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpPost]
        public async Task<IActionResult> CreateAsync(ProjectDTO project, CancellationToken cancellationToken)
        {
            var newProject = await _projectService.CreateAsync(project, _userId, cancellationToken);

            return Ok(newProject.Id);
        }

        /// <summary>
        /// Обновить данные проекта
        /// </summary>
        /// <param name="newProject">Обновленные данные проекта</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Статус операции</returns>
        /// <remarks>
        /// Для обновления проекта необходимо иметь роль владельца
        /// 
        /// Пример запроса:
        /// PUT /api/Projects
        /// {
        ///     "id": 11,
        ///     "name": "project name",
        ///     "description": "project description",
        ///     "security": true
        /// }
        /// </remarks>
        /// <response code="204">Проект успешно обновлен</response>
        /// <response code="401">Недостаточно прав для обновления</response>
        /// <response code="404">Проект не найден</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpPut]
        public async Task<IActionResult> UpdateAsync(ProjectDTO newProject, CancellationToken cancellationToken)
        {
            bool isAuthorize = await _authorizationService.AccessByProjectIdAsync(newProject.Id, _userId, _ownerRoles, cancellationToken);
            if(!isAuthorize)
                return Unauthorized("You havent access to this action");

            try
            {
                await _projectService.UpdateAsync(newProject, cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException) { return NotFound(); }
            catch (Exception) { return StatusCode(500, "Internal server error"); }
        }

        /// <summary>
        /// Удалить проект
        /// </summary>
        /// <param name="projectId">ID проекта для удаления</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Статус операции</returns>
        /// <remarks>
        /// Для удаления проекта необходимо иметь роль владельца
        /// 
        /// Пример запроса:
        /// DELETE /api/Projects?projectId=11
        /// </remarks>
        /// <response code="204">Проект успешно удален</response>
        /// <response code="401">Недостаточно прав для удаления</response>
        /// <response code="404">Проект не найден</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(int projectId, CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByProjectIdAsync(projectId, _userId, _ownerRoles, cancellationToken);
            if(!isAuthorized)
                return Unauthorized("You havent access to this action");

            try
            {
                await _projectService.DeleteAsync(projectId, cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException) { return NotFound(); }
            catch (Exception) { return StatusCode(500, "Internal server error"); }
        }
    }
}
