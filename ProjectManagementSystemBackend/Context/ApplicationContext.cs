using Microsoft.EntityFrameworkCore;
using ProjectManagementSystemBackend.Models;

namespace ProjectManagementSystemBackend.Context
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions options) : base (options) 
        {
            Database.EnsureCreated();
        }
        public DbSet<ActionType> ActionTypes {  get; set; }
        public DbSet<BaseBoard> BaseBoards { get; set; }
        public DbSet<CanbanBoard> CanbanBoards { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<Project> Projects {  get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<ScrumBoard> ScrumBoards { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<TaskHistory> TaskHistories { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
