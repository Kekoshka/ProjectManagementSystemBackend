using FluentValidation;
using ProjectManagementSystemBackend.Models.DTO;

namespace ProjectManagementSystemBackend.Services.FluentValidator.Validators
{
    /// <summary>
    /// Класс для валидации DTO ролей
    /// </summary>
    public class RoleDTOValidator : AbstractValidator<RoleDTO>
    {
        /// <summary>
        /// Конструктор класса для валидации DTO ролей
        /// </summary>
        public RoleDTOValidator()
        {
            RuleFor(r => r.Id).ValidatePositiveNumber();
            RuleFor(r => r.Name).ValidateName();
        }
    }
}
