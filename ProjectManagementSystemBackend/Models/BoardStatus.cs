namespace ProjectManagementSystemBackend.Models
{
    public class BoardStatus
    {
        public int Id { get; set; }
        public int BaseBoardId{ get; set; }
        public int StatusId { get; set; }
        public BaseBoard? BaseBoard { get; set; }
        public Status? Status { get; set; }
        public ICollection<Task> Tasks { get; set; }
    }
}
