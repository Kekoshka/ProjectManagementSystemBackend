namespace ProjectManagementSystemBackend.Models.DTO
{
    public class CommentDTO
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public int ParticipantId { get; set; }
        public int TaskId { get; set; }
    }
}
