using FluentValidation;
using ProjectManagementSystemBackend.Models.DTO;

namespace ProjectManagementSystemBackend.Services.FluentValidator.Validators
{
    /// <summary>
    /// Класс для валидации DTO пользователей
    /// </summary>
    public class UserDTOValidator : AbstractValidator<UserDTO>
    {
        /// <summary>
        /// Конструктор класса для валидации DTO пользователей
        /// </summary>
        public UserDTOValidator()
        {
            RuleFor(u => u.Id).ValidatePositiveNumber();
            RuleFor(u => u.Name).ValidateUserName();
        }
    }
}
