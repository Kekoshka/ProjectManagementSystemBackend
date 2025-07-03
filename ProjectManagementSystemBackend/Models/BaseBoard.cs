namespace ProjectManagementSystemBackend.Models
{
    public class BaseBoard
    {
        public int Id {  get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ProjectId { get; set; }

    }
}
