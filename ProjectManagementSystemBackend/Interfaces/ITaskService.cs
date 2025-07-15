using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystemBackend.Models.DTO;
using ProjectManagementSystemBackend.Services;

namespace ProjectManagementSystemBackend.Interfaces
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskDTO>> GetAsync(int baseBoadrId, CancellationToken cancellationToken);
        Task<TaskDTO> PostAsync(TaskDTO task, int userId, CancellationToken cancellationToken);
        Task UpdateAsync(TaskDTO task, int userId, CancellationToken cancellationToken);
        Task DeleteAsync(int taskId, CancellationToken cancellationToken);
    }
}
