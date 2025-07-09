using ProjectManagementSystemBackend.Interfaces;
using ProjectManagementSystemBackend.Models.DTO;

namespace ProjectManagementSystemBackend.Services
{
    public class TaskService : ITaskService
    {
        public Task DeleteAsync(int taskId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TaskDTO>> GetAsync(int baseBoadrId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<TaskDTO> PostAsync(TaskDTO task, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(TaskDTO task, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
