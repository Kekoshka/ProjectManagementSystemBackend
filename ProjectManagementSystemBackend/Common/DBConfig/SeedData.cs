using Microsoft.EntityFrameworkCore;
using ProjectManagementSystemBackend.Models;

namespace ProjectManagementSystemBackend.Common.DBConfig
{
    /// <summary>
    /// Класс для инициализации начальных данных в БД
    /// </summary>
    public class SeedData
    {
        /// <summary>
        /// Вызов методов для наполнения БД начальными данными
        /// </summary>
        /// <param name="modelBuilder">Построитель модели EFCore</param>
        /// <remarks>
        /// Поочередно вызывает методы и наполняет БД данными о ролях, типах действий и статусах
        /// </remarks>
        public static void Seed(ModelBuilder modelBuilder)
        {
            SeedRoles(modelBuilder);
            SeedActionTypes(modelBuilder);
            SeedStatuses(modelBuilder);
        }
        private static void SeedRoles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
                new Role { Name = "Owner", Id = 1 },
                new Role { Name = "Admin", Id = 2 },
                new Role { Name = "User", Id = 3 });
        }
        private static void SeedActionTypes(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ActionType>()
                .HasData(
                new ActionType { Id = 1, Name = "Create" },
                new ActionType { Id = 2, Name = "Update" },
                new ActionType { Id = 3, Name = "Delete" });
        }
        private static void SeedStatuses(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Status>()
                .HasData(
                new Role { Name = "Новые", Id = 1 },
                new Role { Name = "В работе", Id = 2 },
                new Role { Name = "Проверяются", Id = 3 },
                new Role { Name = "Готовые", Id = 4 });
        }
    }
}
