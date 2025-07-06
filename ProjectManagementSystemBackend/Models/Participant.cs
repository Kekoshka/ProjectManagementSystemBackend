namespace ProjectManagementSystemBackend.Models
{
    public class Participant
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public Project? Project { get; set; }
        public User? User { get; set; }
        public Role? Role { get ; set; }
        public ICollection<TaskHistory>? TaskHistories { get; set; }
        public ICollection<Task>? CreatedTasks { get; set; }
        public ICollection<Task>? ResponsibleTasks { get; set; }
        public ICollection<TaskComment>? TaskComments { get; set; }
    }
}
