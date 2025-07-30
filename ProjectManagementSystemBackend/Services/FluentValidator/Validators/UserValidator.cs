using FluentValidation;
using ProjectManagementSystemBackend.Common.Extensions;
using ProjectManagementSystemBackend.Models;

namespace ProjectManagementSystemBackend.Services.FluentValidator.Validators
{
    /// <summary>
    /// Класс для валидации моделей пользователей
    /// </summary>
    public class UserValidator : AbstractValidator<User>
    {
        /// <summary>
        /// Конструктор класса для валидации моделей пользователей
        /// </summary>
        public UserValidator()
        {
            RuleFor(u => u.Id).ValidatePositiveNumber();
            RuleFor(u => u.Name).ValidateUserName();
            RuleFor(u => u.Login).ValidateLogin();
            RuleFor(u => u.Password).ValidatePassword();
        }
    }
}
