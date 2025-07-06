namespace ProjectManagementSystemBackend.Models
{
    public class TaskComment
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public int ParticipantId { get; set; }
        public int TaskId {  get; set; }
        public Participant? Participant { get; set; }
        public Task? Task { get; set; }
    }
}
