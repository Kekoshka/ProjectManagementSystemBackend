using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystemBackend.Models;
using ProjectManagementSystemBackend.Models.DTO;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagementSystemBackend.Interfaces
{
    public interface IStatusService
    {
        Task<IEnumerable<StatusDTO>> GetAsync(int baseBoardId, CancellationToken cancellationToken);
        Task<StatusDTO> PostAsync(StatusDTO status, CancellationToken cancellationToken);
        Task UpdateAsync(StatusDTO status, CancellationToken cancellationToken);
        Task DeleteAsync(int boardStatusId, CancellationToken cancellationToken);
        Task CreateBaseStatusesAsync(int baseBoardId, CancellationToken cancellationToken);
    }
}
