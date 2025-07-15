using Mapster;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Interfaces;
using ProjectManagementSystemBackend.Models;
using ProjectManagementSystemBackend.Models.DTO;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagementSystemBackend.Services
{
    public class ParticipantService : IParticipantService
    {
        ApplicationContext _context;
        int[] _userRoles = [1, 2, 3];
        int _ownerRole = 1;

        public ParticipantService(ApplicationContext context) 
        {
            _context = context;
        }

        public async Task CreateBaseParticipantAsync(int projectId, int userId, CancellationToken cancellationToken)
        {

            Participant newParticipant = new()
            {
                ProjectId = projectId,
                UserId = userId,
                RoleId = _ownerRole,
            };
            await _context.Participants.AddAsync(newParticipant);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(int participantId, CancellationToken cancellationToken)
        {
            var participant = await _context.Participants
                .FirstOrDefaultAsync(p => p.Id == participantId, cancellationToken);
            if (participant is null)
                throw new KeyNotFoundException();
            _context.Participants.Remove(participant);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<ParticipantDTO>> GetAsync(int projectId, CancellationToken cancellationToken)
        {
            var participants = await _context.Participants
                .Include(p => p.User)
                .Where(p => p.ProjectId == projectId)
                .ProjectToType<ParticipantDTO>()
                .ToListAsync(cancellationToken);
            return participants;
        }

        public async Task<ParticipantDTO> PostAsync(ParticipantDTO participant, CancellationToken cancellationToken)
        {
            if (!_userRoles.Any(r => r == participant.RoleId))
                throw new InvalidDataException("Invalid role id");
            var existUser = await _context.Users.FindAsync(participant.UserId, cancellationToken);
            if (existUser is null)
                throw new InvalidDataException("Invalid user id");

            var existParticipant = await _context.Participants
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProjectId == participant.ProjectId &&
                    p.UserId == participant.UserId, cancellationToken);
            if (existParticipant is not null)
                throw new InvalidOperationException("User is already a participant of the project");

            Participant newParticipant = participant.Adapt<Participant>();
            await _context.Participants.AddAsync(newParticipant, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return newParticipant.Adapt<ParticipantDTO>();
        }

        public async Task UpdateAsync(ParticipantDTO newParticipant, CancellationToken cancellationToken)
        {
            if (!_userRoles.Any(r => r == newParticipant.RoleId))
                throw new InvalidDataException("Invalid role id");

            var participant = await _context.Participants.FindAsync(newParticipant.Id);
            if (participant is null)
                throw new InvalidDataException($"Participant with id {newParticipant.Id} id not found");

            participant.RoleId = newParticipant.RoleId;
            await _context.SaveChangesAsync();
        }
     
    }
}
