using FluentValidation;
using ProjectManagementSystemBackend.Models.DTO;

namespace ProjectManagementSystemBackend.Services.FluentValidator.Validators
{
    /// <summary>
    /// Класс для валидации DTO комментариев
    /// </summary>
    public class CommentDTOValidator : AbstractValidator<CommentDTO>
    {
        /// <summary>
        /// Конструктор класса для валидации DTO комментариев
        /// </summary>
        public CommentDTOValidator()
        {
            RuleFor(c => c.Id).ValidatePositiveNumber();
            RuleFor(c => c.ParticipantId).ValidatePositiveNumber();
            RuleFor(c => c.TaskId).ValidatePositiveNumber();
            RuleFor(c => c.Message).ValidateMessage();
        }
    }
}
