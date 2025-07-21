using Microsoft.EntityFrameworkCore;
using ProjectManagementSystemBackend.Common.DBConfig;
using ProjectManagementSystemBackend.Models;
using System.Reflection;

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
        /// Заполнение начальных данных
        /// </remarks>
        public DbSet<User> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            SeedData.Seed(modelBuilder);   
        }
    }
}
