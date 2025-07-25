using FluentValidation;
using ProjectManagementSystemBackend.Models.DTO;

namespace ProjectManagementSystemBackend.Services.FluentValidator.Validators
{
    /// <summary>
    /// Класс для валидации DTO базовых досок
    /// </summary>
    public class BaseBoardDTOValidator : AbstractValidator<BaseBoardDTO>
    {
        /// <summary>
        /// Конструктор класса для валидации DTO базовых досок
        /// </summary>
        public BaseBoardDTOValidator()
        {
            RuleFor(bb => bb.Name).ValidateName();
            RuleFor(bb => bb.Description).ValidateDescription();
            RuleFor(bb => bb.Id).ValidatePositiveNumber();
            RuleFor(bb => bb.ProjectId).ValidatePositiveNumber();
        }
    }
}
