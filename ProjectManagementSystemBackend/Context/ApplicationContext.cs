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
        public DbSet<BoardStatus> BoardStatuses { get; set; }
        public DbSet<CanbanBoard> CanbanBoards { get; set; }
        public DbSet<TaskComment> TaskComments { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<Project> Projects {  get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<ScrumBoard> ScrumBoards { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Models.Task> Tasks { get; set; }
        public DbSet<TaskHistory> TaskHistories { get; set; }
        public DbSet<User> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Participant>()
                .HasMany(p => p.CreatedTasks)
                .WithOne(t => t.Creator)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Participant>()
                .HasMany(p => p.ResponsibleTasks)
                .WithOne(t => t.ResponsiblePerson)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Role>()
                .HasData(
                new Role { Name = "Owner", Id = 1 },
                new Role { Name = "Admin", Id = 2 },
                new Role { Name = "User", Id = 3 });

            modelBuilder.Entity<Status>()
                .HasData(
                new Role { Name = "Новые", Id = 1 },
                new Role { Name = "В работе", Id = 2 },
                new Role { Name = "Проверяются", Id = 3 },
                new Role { Name = "Готовые", Id = 4 });

            modelBuilder.Entity<ActionType>()
                .HasData(
                new ActionType { Id = 1, Name = "Create" },
                new ActionType { Id = 2, Name = "Update" },
                new ActionType { Id = 3, Name = "Delete" });
        }
    }
}
