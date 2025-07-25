using FluentValidation;
using ProjectManagementSystemBackend.Models.DTO;

namespace ProjectManagementSystemBackend.Services.FluentValidator.Validators
{
    /// <summary>
    /// Класс для валидации DTO канбан досок
    /// </summary>
    public class KanbanBoardDTOValidator : AbstractValidator<KanbanBoardDTO>
    {
        /// <summary>
        /// Конструктор класса для валидации DTO канбан досок
        /// </summary>
        public KanbanBoardDTOValidator()
        {
            RuleFor(kb => kb.Id).ValidatePositiveNumber();
            RuleFor(kb => kb.TaskLimit)
                .GreaterThan(0).WithMessage("Number must be a positive number");

            RuleFor(kb => kb.BaseBoardId).ValidatePositiveNumber();
            RuleFor(kb => kb.BaseBoard)
                .NotNull().When(kb => kb.BaseBoardId > 0)
                .WithMessage("BaseBoard must be specified as the BaseBoardId is specified")
                .SetValidator(new BaseBoardDTOValidator());
        }
    }
}
