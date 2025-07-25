using FluentValidation;
using ProjectManagementSystemBackend.Models.DTO;

namespace ProjectManagementSystemBackend.Services.FluentValidator.Validators
{
    /// <summary>
    /// Класс для валидации данных авторизации
    /// </summary>
    public class AuthDataValidator : AbstractValidator<AuthData>
    {
        /// <summary>
        /// Конструктор класса для валидации данных авторизации
        /// </summary>
        public AuthDataValidator()
        {
            RuleFor(ad => ad.Login).ValidateLogin();
            RuleFor(ad => ad.Password).ValidatePassword();
        }
    }
}
