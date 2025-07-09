using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystemBackend.Models.DTO;

namespace ProjectManagementSystemBackend.Interfaces
{
    public interface IParticipantService
    {
        Task<IEnumerable<ParticipantDTO>> GetAsync(int projectId, CancellationToken cancellationToken);
        Task<ParticipantDTO> PostAsync(ParticipantDTO participant, CancellationToken cancellationToken);
        Task UpdateAsync(ParticipantDTO participant, CancellationToken cancellationToken);
        Task DeleteAsync(int participantId, CancellationToken cancellationToken);


    }
}
