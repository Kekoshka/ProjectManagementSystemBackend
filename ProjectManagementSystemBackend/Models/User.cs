namespace ProjectManagementSystemBackend.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }    
        public ICollection<Participant>? Participants { get; set; }
    }
}
