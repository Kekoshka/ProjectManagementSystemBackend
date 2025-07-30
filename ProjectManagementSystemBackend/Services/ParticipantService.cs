using Mapster;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Interfaces;
using ProjectManagementSystemBackend.Models;
using ProjectManagementSystemBackend.Models.DTO;
using ProjectManagementSystemBackend.Common.CustomExceptions;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagementSystemBackend.Services
{
    /// <summary>
    /// Сервис для управления участниками
    /// Позволяет получать, создавать, изменять и удалять участников проектов
    /// </summary>
    public class ParticipantService : IParticipantService
    {
        TypeAdapterConfig config = new TypeAdapterConfig();
        ApplicationContext _context;
        int[] _userRoles = [1, 2, 3];
        int _ownerRole = 1;

        /// <summary>
        /// Конструктор сервиса участников проекта 
        /// </summary>
        /// <param name="context">Контекст базы данных</param>
        public ParticipantService(ApplicationContext context) 
        {
            _context = context;
        }

        /// <summary>
        /// Создать базового участника проекта
        /// </summary>
        /// <param name="projectId">ID проекта</param>
        /// <param name="userId">ID пользователя</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <remarks>
        /// Создает базового участника проекта, вызывается при создании проекта,
        /// наделяет участника ролью владельца проекта
        /// </remarks>
        public async Task CreateBaseParticipantAsync(int projectId, int userId, CancellationToken cancellationToken)
        {
            Participant newParticipant = new()
            {
                ProjectId = projectId,
                UserId = userId,
                RoleId = _ownerRole,
            };
            await _context.Participants.AddAsync(newParticipant, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Удалить участника проекта
        /// </summary>
        /// <param name="participantId">ID участника проекта</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <exception cref="NotFoundException">Если участник не был найден</exception>
        public async Task DeleteAsync(int participantId, CancellationToken cancellationToken)
        {
            var participant = await _context.Participants
                .FirstOrDefaultAsync(p => p.Id == participantId, cancellationToken);
            if (participant is null)
                throw new NotFoundException();
            _context.Participants.Remove(participant);
            await _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Получить участников проекта
        /// </summary>
        /// <param name="projectId">ID проекта</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Список участников проекта</returns>
        public async Task<IEnumerable<ParticipantDTO>> GetAsync(int projectId, CancellationToken cancellationToken)
        {
            var participants = await _context.Participants
                .Include(p => p.User)
                .Where(p => p.ProjectId == projectId)
                .ProjectToType<ParticipantDTO>()
                .ToListAsync(cancellationToken);
            return participants;
        }

        /// <summary>
        /// Создать нового участника проекта
        /// </summary>
        /// <param name="participant">DTO с данными нового участника проекта</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>DTO с данными созданного участника проекта</returns>
        /// <exception cref="BadRequestException">Если был передан несуществующий Id пользователя или роли</exception>
        /// <exception cref="ConflictException">Если пользователь уже является участником проекта</exception>
        public async Task<ParticipantDTO> PostAsync(ParticipantDTO participant, CancellationToken cancellationToken)
        {
            if (!_userRoles.Any(r => r == participant.RoleId))
                throw new BadRequestException("Invalid role id");
            var existUser = await _context.Users.FindAsync(participant.UserId, cancellationToken);
            if (existUser is null)
                throw new BadRequestException("Invalid user id");

            var existParticipant = await _context.Participants
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProjectId == participant.ProjectId &&
                    p.UserId == participant.UserId, cancellationToken);
            if (existParticipant is not null)
                throw new ConflictException("User is already a participant of the project");

            Participant newParticipant = participant.Adapt<Participant>(config.Fork(f => f.ForType<ParticipantDTO,Participant>().Ignore("Id").Ignore("User")));
            await _context.Participants.AddAsync(newParticipant, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return newParticipant.Adapt<ParticipantDTO>();
        }

        /// <summary>
        /// Обновить данные участника проекта
        /// </summary>
        /// <param name="newParticipant">DTO с обновленными данными участника</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <remarks>
        /// Обновлет данные о роли участника проекта
        /// </remarks>
        /// <exception cref="BadRequestException">Если передан неверный ID роли или участника</exception>
        public async Task UpdateAsync(ParticipantDTO newParticipant, CancellationToken cancellationToken)
        {
            if (!_userRoles.Any(r => r == newParticipant.RoleId))
                throw new BadRequestException("Invalid role id");

            var participant = await _context.Participants.FindAsync(newParticipant.Id, cancellationToken);
            if (participant is null)
                throw new BadRequestException($"Participant with id {newParticipant.Id} id not found");

            participant.RoleId = newParticipant.RoleId;
            await _context.SaveChangesAsync(cancellationToken);
        }
     
    }
}
