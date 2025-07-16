namespace ProjectManagementSystemBackend.Models
{
    public class Task
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; }
        public DateTime LastUpdate { get; set; }
        public DateTime TimeLimit { get; set; }
        public int CreatorId { get; set; }
        public int ResponsiblePersonId { get; set; }
        public int BoardStatusId { get; set; }
        public BoardStatus? BoardStatus { get; set; }
        public Participant? ResponsiblePerson { get; set; }
        public Participant? Creator { get; set; }
        public ICollection<TaskComment>? TaskComments { get; set; }
        public Task() { }
        public Task(string name, string description, int priority, DateTime lastUpdate, DateTime timeLimit, int creatorId, int responsiblePersonId, int boardStatusId)
        {
            Name = name;
            Description = description;
            Priority = priority;
            LastUpdate = lastUpdate;
            TimeLimit = timeLimit;
            CreatorId = creatorId;
            ResponsiblePersonId = responsiblePersonId;
            BoardStatusId = boardStatusId;
        }
    }
}
