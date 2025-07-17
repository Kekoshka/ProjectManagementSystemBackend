using Microsoft.EntityFrameworkCore;
using ProjectManagementSystemBackend.Models;

namespace ProjectManagementSystemBackend.Context
{
    /// <summary>
    /// Контекст базы данных системы управления проектами
    /// </summary>
    /// <remarks>
    /// Представляет сессию с базой данных и обеспечивает:
    /// - Доступ к сущностям системы через DbSet-свойства
    /// - Конфигурацию модели данных
    /// - Инициализацию начальных данных
    /// </remarks>
    public class ApplicationContext : DbContext
    {
        /// <summary>
        /// Конструктор контекста базы данных
        /// </summary>
        /// <param name="options">Параметры конфигурации контекста</param>
        /// <remarks>
        /// Автоматически создает базу данных при первом обращении, если она не существует
        /// </remarks>
        public ApplicationContext(DbContextOptions options) : base (options) 
        {
            Database.EnsureCreated();
        }
        /// <summary>
        /// Типы действий в истории задач
        /// </summary>
        public DbSet<ActionType> ActionTypes { get; set; }

        /// <summary>
        /// Базовые доски проектов
        /// </summary>
        public DbSet<BaseBoard> BaseBoards { get; set; }

        /// <summary>
        /// Статусы досок
        /// </summary>
        public DbSet<BoardStatus> BoardStatuses { get; set; }

        /// <summary>
        /// Kanban-доски
        /// </summary>
        public DbSet<KanbanBoard> KanbanBoards { get; set; }

        /// <summary>
        /// Комментарии к задачам
        /// </summary>
        public DbSet<TaskComment> TaskComments { get; set; }

        /// <summary>
        /// Участники проектов
        /// </summary>
        public DbSet<Participant> Participants { get; set; }

        /// <summary>
        /// Проекты
        /// </summary>
        public DbSet<Project> Projects { get; set; }

        /// <summary>
        /// Роли пользователей
        /// </summary>
        public DbSet<Role> Roles { get; set; }

        /// <summary>
        /// Scrum-доски
        /// </summary>
        public DbSet<ScrumBoard> ScrumBoards { get; set; }

        /// <summary>
        /// Статусы задач
        /// </summary>
        public DbSet<Status> Statuses { get; set; }

        /// <summary>
        /// Задачи
        /// </summary>
        public DbSet<Models.Task> Tasks { get; set; }

        /// <summary>
        /// История изменений задач
        /// </summary>
        public DbSet<TaskHistory> TaskHistories { get; set; }

        /// <summary>
        /// Конфигурация модели данных
        /// </summary>
        /// <param name="modelBuilder">Построитель модели данных</param>
        /// <remarks>
        /// 
        /// Настройка связей между сущностями
        /// Заполнение начальных данных:
        /// Роли пользователей (Owner, Admin, User)
        /// Статусы задач (Новые, В работе, Проверяются, Готовые)
        /// Типы действий (Create, Update, Delete)
        /// </remarks>
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
