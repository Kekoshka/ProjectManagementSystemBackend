namespace ProjectManagementSystemBackend.Models.DTO
{
    public class TaskHistoryDTO
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Action { get; set; }
        public string UserName { get; set; }
        public int TaskId { get; set; }
        public string ActionTypeName { get; set; }
    }
}
