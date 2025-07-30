using BCrypt.Net;
using Microsoft.AspNetCore.Identity;
using ProjectManagementSystemBackend.Common.CustomExceptions;
using ProjectManagementSystemBackend.Interfaces;

namespace ProjectManagementSystemBackend.Services
{
    /// <summary>
    /// Сервис для хеширования паролей
    /// </summary>
    public class PasswordHasherService : IPasswordHasherService
    {
        private readonly int _workFactor = 14;
        /// <summary>
        /// Захешировать пароль
        /// </summary>
        /// <param name="password">Пароль</param>
        /// <returns>Захешированная строка пароля</returns>
        public string Hash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, _workFactor);
        }
        /// <summary>
        /// Сверить пароль и хеш
        /// </summary>
        /// <param name="text">Пароль</param>
        /// <param name="hash">Хеш</param>
        /// <returns>
        /// True: если пароль соответствует хешу
        /// False: если пароль не соответствует хешу
        /// </returns>
        public bool Verify(string text, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(text, hash);
        }
    }
}
