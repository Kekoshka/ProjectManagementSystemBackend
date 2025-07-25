using FluentValidation;
using ProjectManagementSystemBackend.Models.DTO;

namespace ProjectManagementSystemBackend.Services.FluentValidator.Validators
{
    /// <summary>
    /// Класс для валидации DTO скрам досок
    /// </summary>
    public class ScrumBoardDTOValidator : AbstractValidator<ScrumBoardDTO>
    {
        /// <summary>
        /// Конструктор класса для валидации DTO скрам досок
        /// </summary>
        public ScrumBoardDTOValidator()
        {
            RuleFor(sb => sb.Id).ValidatePositiveNumber();
            RuleFor(sb => sb.BaseBoardId).ValidatePositiveNumber();
            RuleFor(sb => sb.TimeLimit).ValidateFutureDate();
            RuleFor(sb => sb.BaseBoard)
                .NotNull().When(sb => sb.BaseBoardId > 0)
                .WithMessage("BaseBoard must be specified as the BaseBoardId is specified")
                .SetValidator(new BaseBoardDTOValidator());
        }
    }
}
