namespace ProjectManagementSystemBackend.Interfaces
{
    public interface ITaskHistory
    {
        Task CreateAsync(Models.Task task, int userId);
        Task UpdateAsync(Models.Task oldTask, Models.Task newTask, int userId);
        Task DeleteAsync(Models.Task task, int userId);
    }
}
