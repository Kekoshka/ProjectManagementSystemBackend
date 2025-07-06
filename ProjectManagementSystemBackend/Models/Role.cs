namespace ProjectManagementSystemBackend.Models
{
    public class Role
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public ICollection<Participant>? Participants { get; set; }

        //1 - Owner, 2 - Admin, 3 - User
    }
}
