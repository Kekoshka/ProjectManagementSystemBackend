namespace ProjectManagementSystemBackend.Models
{
    public class Participant
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
    }
}
