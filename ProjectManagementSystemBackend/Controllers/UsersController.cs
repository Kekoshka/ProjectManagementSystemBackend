using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Interfaces;
using ProjectManagementSystemBackend.Models;
using ProjectManagementSystemBackend.Models.DTO;


namespace ProjectManagementSystemBackend.Controllers
{
    /// <summary>
    /// Контроллер для авторизации и регистрации пользователей
    /// </summary>
    /// <remarks>
    /// Позволяет регистрировать и авторизовывать пользователей системы
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        IUserService _userService;
        /// <summary>
        /// Конструктор контроллера пользователей
        /// </summary>
        /// <param name="userService">Сервис пользователей</param>
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }
        /// <summary>
        /// Авторизовать пользователя и получить jwt токен
        /// </summary>
        /// <param name="authData">Данные для авторизации</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>jwt токен</returns>
        /// <remarks>
        /// Пример запроса:
        /// GET /api/Users/authorization
        /// {
        ///     "login": "UserLogin",   
        ///     "password": "UserP@ssw0rd"
        /// }
        /// </remarks>
        /// <response code="200">авторизация прошла успешно, jwt получен</response>
        /// <response code="401">Пользователь с такими данными для авторизации не найден</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpPost("authorization")]
        public async Task<IActionResult> AuthorizationAsync(AuthData authData, CancellationToken cancellationToken)
        {
            try
            {
                string jwt = await _userService.AuthorizationAsync(authData, cancellationToken);
                return Ok(jwt);
            }
            catch (UnauthorizedAccessException ex) { return Unauthorized(ex.Message); }
            catch (Exception) { return StatusCode(500, "Internal server error"); }

        }

        /// <summary>
        /// Зарегистрировать пользователя
        /// </summary>
        /// <param name="user">Данные для регистрации</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>статус операции</returns>
        /// <remarks>
        /// Пример запроса:
        /// GET /api/Users/registration
        /// {
        ///     "id": 0,
        ///     "name": "UserName",
        ///     "login": "UserLogin",
        ///     "password": "UserP@ssw0rd",
        /// }
        /// </remarks>
        /// <response code="204">Регистрация прошла успешно</response>
        /// <response code="409">Пользователь с таким логином уже существует</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpPost("registration")]
        public async Task<IActionResult> RegistrationAsync(User user, CancellationToken cancellationToken)
        {
            try
            {
                await _userService.RegistrationAsync(user, cancellationToken);
                return NoContent();
            }
            catch (InvalidOperationException ex) { return Conflict(ex.Message); }
            catch (Exception) { return StatusCode(500, "Internal server error"); }
        }
    }
}
