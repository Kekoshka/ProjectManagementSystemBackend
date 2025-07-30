using FluentValidation;
using ProjectManagementSystemBackend.Common.Extensions;
using ProjectManagementSystemBackend.Models.DTO;

namespace ProjectManagementSystemBackend.Services.FluentValidator.Validators
{
    /// <summary>
    /// Класс для валидации DTO проектов
    /// </summary>
    public class ProjectDTOValidator : AbstractValidator<ProjectDTO>
    {
        /// <summary>
        /// Конструктор класса для валидации DTO проектов
        /// </summary>
        public ProjectDTOValidator()
        {
            RuleFor(p => p.Id).ValidatePositiveNumber();
            RuleFor(p => p.Name).ValidateName();
            RuleFor(p => p.Description).ValidateDescription();
        }
    }
}
