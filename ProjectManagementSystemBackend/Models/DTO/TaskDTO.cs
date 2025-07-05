namespace ProjectManagementSystemBackend.Models.DTO
{
    public class TaskDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; }
        public DateTime LastUpdate { get; set; }
        public DateTime TimeLimit { get; set; }
        public int CreatorId { get; set; }
        public int ResponsiblePersonId { get; set; }
        public int BoardStatusId { get; set; }
    }
}
