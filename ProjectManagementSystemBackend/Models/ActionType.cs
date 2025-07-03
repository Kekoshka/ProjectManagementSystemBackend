namespace ProjectManagementSystemBackend.Models
{
    public class ActionType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<TaskHistory>? TaskHistories { get; set; }
    }
}
