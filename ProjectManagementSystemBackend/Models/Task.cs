namespace ProjectManagementSystemBackend.Models
{
    public class Task
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; }
        public DateTime LastUpdate { get; set; }
        public DateTime TimeLimit { get; set; }
        public int CreatorId { get; set; }
        public int ResponsiblePersonId { get; set; }
        public int StatusId { get; set; }
        public Status Status { get; set; }
        public Participant ResponsiblePerson { get; set; }
        public Participant Creator { get; set; }

    }
}
