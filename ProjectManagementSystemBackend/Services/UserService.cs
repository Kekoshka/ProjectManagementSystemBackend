using Microsoft.EntityFrameworkCore;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Interfaces;
using ProjectManagementSystemBackend.Models;
using ProjectManagementSystemBackend.Models.DTO;
using ProjectManagementSystemBackend.Common.CustomExceptions;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagementSystemBackend.Services
{
    /// <summary>
    /// Сервис для работы с пользователями системы
    /// </summary>
    /// <remarks>
    /// Предоставляет функционал для регистрации и авторизации пользователей.
    /// </remarks>
    public class UserService : IUserService
    {
        ApplicationContext _context; 
        IAuthenticationService _authenticationService; 
        IPasswordHasherService _passwordHasherService;

        /// <summary>
        /// Конструктор сервиса пользователей
        /// </summary>
        /// <param name="context">Контекст базы данных</param>
        /// <param name="authenticationService">Сервис аутентификации</param>
        /// <param name="passwordHasherService">Сервис хеширования паролей</param>
        public UserService(ApplicationContext context, IAuthenticationService authenticationService, IPasswordHasherService passwordHasherService) 
        {
            _context = context;
            _authenticationService = authenticationService;
            _passwordHasherService = passwordHasherService;
        }

        /// <summary>
        /// Авторизация пользователя в системе
        /// </summary>
        /// <param name="authData">Данные для авторизации (логин и пароль)</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>JWT токен</returns>
        /// <exception cref="UnauthorizedException">Если неверный логин или пароль</exception>
        /// <remarks>
        /// Процесс авторизации:
        /// 1. Поиск пользователя по логину
        /// 2. Проверка соответствия пароля
        /// 3. Генерация JWT-токена при успешной проверке
        /// </remarks>
        public async Task<string> AuthorizationAsync(AuthData authData, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == authData.Login, cancellationToken);
            if (user is null)
                throw new UnauthorizedException("Invalid login or password");

            bool passwordIsValid = _passwordHasherService.Verify(authData.Password, user.Password);
            if (!passwordIsValid)
                throw new UnauthorizedException("Invalid login or password");

            var jwt = _authenticationService.GetJWT(user);
            return jwt;
        }

        /// <summary>
        /// Регистрация нового пользователя
        /// </summary>
        /// <param name="user">Данные нового пользователя</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Task</returns>
        /// <exception cref="ConflictException">Если пользователь с таким логином уже существует</exception>
        /// <remarks>
        /// Процесс регистрации:
        /// 1. Проверка уникальности логина
        /// 2. Хеширование пароля
        /// 3. Сохранение нового пользователя в БД
        /// </remarks>
        public async Task RegistrationAsync(User user, CancellationToken cancellationToken)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Login == user.Login, cancellationToken);
            if (existingUser is not null)
                throw new ConflictException("user with such data is already exists");

            User newUser = new()
            {
                Login = user.Login,
                Name = user.Name,
                Password = _passwordHasherService.Hash(user.Password)
            };
            await _context.Users.AddAsync(newUser, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

        }
    }
}
