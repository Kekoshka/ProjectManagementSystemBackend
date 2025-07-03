namespace ProjectManagementSystemBackend.Models
{
    public class Status
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<BaseBoard>? BaseBoards { get; set; }
        public ICollection<Task>? Tasks { get; set; }
    }
}
