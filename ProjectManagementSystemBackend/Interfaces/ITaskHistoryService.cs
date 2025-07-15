namespace ProjectManagementSystemBackend.Interfaces
{
    public interface ITaskHistoryService
    {
        Task CreateAsync(Models.Task task, int userId,CancellationToken cancellationToken);
        Task UpdateAsync(Models.Task oldTask, Models.Task newTask, int userId, CancellationToken cancellationToken);
    }
}
