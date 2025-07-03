namespace ProjectManagementSystemBackend.Models
{
    public class BoardBase
    {
        public int Id {  get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ProjectId { get; set; }

    }
}
