namespace ProjectManagementSystemBackend.Models.DTO
{
    public class KanbanBoardDTO
    {
        public int Id { get; set; }
        public int TaskLimit { get; set; }
        public int BaseBoardId { get; set; }
        public BaseBoardDTO BaseBoard { get; set; }
    }
}
