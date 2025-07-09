using ProjectManagementSystemBackend.Interfaces;
using ProjectManagementSystemBackend.Models.DTO;

namespace ProjectManagementSystemBackend.Services
{
    public class ParticipantService : IParticipantService
    {
        public Task DeleteAsync(int participantId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ParticipantDTO>> GetAsync(int projectId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ParticipantDTO> PostAsync(ParticipantDTO participant, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(ParticipantDTO participant, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
