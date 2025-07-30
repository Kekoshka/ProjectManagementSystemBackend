using FluentValidation;
using ProjectManagementSystemBackend.Common.Extensions;
using ProjectManagementSystemBackend.Models.DTO;

namespace ProjectManagementSystemBackend.Services.FluentValidator.Validators
{
    /// <summary>
    /// Класс для валидации DTO статусов
    /// </summary>
    public class StatusDTOValidator : AbstractValidator<StatusDTO>
    {
        /// <summary>
        /// Конструктор класса для валидации DTO статусов
        /// </summary>
        public StatusDTOValidator()
        {
            RuleFor(s => s.Id).ValidatePositiveNumber();
            RuleFor(s => s.StatusId).ValidatePositiveNumber();
            RuleFor(s => s.BaseBoardId).ValidatePositiveNumber();
            RuleFor(s => s.Name).ValidateName();
        }
    }
}
