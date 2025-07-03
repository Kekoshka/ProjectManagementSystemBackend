namespace ProjectManagementSystemBackend.Models.DTO
{
    public class ScrumBoardDTO
    {
        public int Id { get; set; }
        public DateTime TimeLimit { get; set; }
        public int BaseBoardId { get; set; }
        public BaseBoard BaseBoard { get; set; }
    }
}
