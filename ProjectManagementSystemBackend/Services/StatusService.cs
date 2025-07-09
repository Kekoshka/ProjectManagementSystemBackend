using ProjectManagementSystemBackend.Interfaces;
using ProjectManagementSystemBackend.Models.DTO;

namespace ProjectManagementSystemBackend.Services
{
    public class StatusService : IStatusService
    {
        public Task DeleteAsync(int boardStatusId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<StatusDTO>> GetAsync(int baseBoardId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<StatusDTO> PostAsync(StatusDTO status, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(StatusDTO status, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
