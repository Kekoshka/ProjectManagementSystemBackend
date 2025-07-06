namespace ProjectManagementSystemBackend.Models
{
    public class TaskHistory
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Action { get; set; }
        public int UserId { get; set; }
        public int TaskId { get; set; }
        public int ActionTypeId { get; set; }
        public User? User { get; set; }
        public Task? Task { get; set; }
        public ActionType? ActionType { get; set; }
        public TaskHistory() { }
        public TaskHistory(DateTime date, string action, int userId, int taskId, int actionTypeId)
        {
            Date = date;
            Action = action;
            UserId = userId;
            TaskId = taskId;
            ActionTypeId = actionTypeId;
        }
    }
}
