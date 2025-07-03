namespace ProjectManagementSystemBackend.Models.DTO
{
    public class ParticipantDTO
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public UserDTO User { get; set; }
    }
}
