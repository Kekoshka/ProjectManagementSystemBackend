namespace ProjectManagementSystemBackend.Models
{
    public class ScrumBoard
    {
        public int Id { get; set; }
        public DateTime TimeLimit { get; set; }
        public int BaseBoardId { get; set; }
        public BaseBoard? BaseBoard { get; set; }
    }
}
