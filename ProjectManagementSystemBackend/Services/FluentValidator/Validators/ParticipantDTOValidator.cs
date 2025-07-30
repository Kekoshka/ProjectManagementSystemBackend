using FluentValidation;
using ProjectManagementSystemBackend.Common.Extensions;
using ProjectManagementSystemBackend.Models.DTO;

namespace ProjectManagementSystemBackend.Services.FluentValidator.Validators
{
    /// <summary>
    /// Класс для валидации DTO участников
    /// </summary>
    public class ParticipantDTOValidator : AbstractValidator<ParticipantDTO>
    {
        /// <summary>
        /// Конструктор класса для валидации DTO участников
        /// </summary>
        public ParticipantDTOValidator()
        {
            RuleFor(p => p.Id).ValidatePositiveNumber();
            RuleFor(p => p.UserId).ValidatePositiveNumber();
            RuleFor(p => p.RoleId).ValidatePositiveNumber();
            RuleFor(p => p.ProjectId).ValidatePositiveNumber();
            RuleFor(p => p.User)
                .NotNull().When(p => p.UserId > 0)
                .WithMessage("User must be specified as the UserId is specified")
                .SetValidator(new UserDTOValidator());
        }
    }
}
