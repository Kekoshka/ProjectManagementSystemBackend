using FluentValidation;
using ProjectManagementSystemBackend.Common.Extensions;
using ProjectManagementSystemBackend.Models.DTO;

namespace ProjectManagementSystemBackend.Services.FluentValidator.Validators
{
    /// <summary>
    /// Класс для валидации DTO истории задач
    /// </summary>
    public class TaskHistoryDTOValidator : AbstractValidator<TaskHistoryDTO>
    {
        /// <summary>
        /// Конструктор класса для валидации DTO истории задач
        /// </summary>
        public TaskHistoryDTOValidator()
        {
            RuleFor(t => t.Id).ValidatePositiveNumber();
            RuleFor(t => t.TaskId).ValidatePositiveNumber();
            RuleFor(t => t.UserName).ValidateUserName();
            RuleFor(t => t.Date).ValidateFutureDate();
            RuleFor(t => t.ActionTypeName).ValidateName();
            RuleFor(t => t.Action).ValidateAction();
        }
    }
}
