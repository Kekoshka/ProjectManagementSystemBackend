namespace ProjectManagementSystemBackend.Models
{
    public class CanbanBoard
    {
        public int Id { get; set; }
        public int TaskLimit { get; set; }
        public int BaseBoardId { get; set; }
        public BaseBoard? BaseBoard { get; set; }
    }
}
