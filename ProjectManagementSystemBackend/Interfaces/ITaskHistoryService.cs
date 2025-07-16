using ProjectManagementSystemBackend.Models;
using ProjectManagementSystemBackend.Models.DTO;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagementSystemBackend.Interfaces
{
    public interface ITaskHistoryService
    {
        Task<IEnumerable<TaskHistoryDTO>> GetAsync(int taskId, CancellationToken cancellationToken);
        Task CreateAsync(Models.Task task, int userId,CancellationToken cancellationToken);
        Task UpdateAsync(Models.Task oldTask, Models.Task newTask, int userId, CancellationToken cancellationToken);
    }
}
