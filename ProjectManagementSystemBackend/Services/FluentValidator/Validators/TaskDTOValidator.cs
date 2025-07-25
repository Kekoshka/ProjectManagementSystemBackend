using FluentValidation;
using ProjectManagementSystemBackend.Models.DTO;

namespace ProjectManagementSystemBackend.Services.FluentValidator.Validators
{
    /// <summary>
    /// Класс для валидации DTO задач
    /// </summary>
    public class TaskDTOValidator : AbstractValidator<TaskDTO>
    {
        /// <summary>
        /// Конструктор класса для валидации DTO задач
        /// </summary>
        public TaskDTOValidator()
        {
            RuleFor(t => t.Id).ValidatePositiveNumber();
            RuleFor(t => t.ResponsiblePersonId).ValidatePositiveNumber();
            RuleFor(t => t.BoardStatusId).ValidatePositiveNumber();
            RuleFor(t => t.CreatorId).ValidatePositiveNumber();
            RuleFor(t => t.Name).ValidateName();
            RuleFor(t => t.Description).ValidateDescription();
            RuleFor(t => t.LastUpdate).ValidateFutureDate();
            RuleFor(t => t.TimeLimit).ValidateFutureDate();
            RuleFor(t => t.Priority).ValidatePriority();

        }
    }
}
