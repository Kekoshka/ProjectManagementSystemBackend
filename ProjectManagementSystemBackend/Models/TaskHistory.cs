namespace ProjectManagementSystemBackend.Models
{
    public class TaskHistory
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Action { get; set; }
        public int ParticipantId { get; set; }
        public int TaskId { get; set; }
        public int ActionTypeId { get; set; }
    }
}
