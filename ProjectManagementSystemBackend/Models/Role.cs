namespace ProjectManagementSystemBackend.Models
{
    /// <summary>
    /// Роль участника проекта
    /// </summary>
    /// <remarks>
    /// Опрелеляет уровень доступа и привилегии пользователя в системе
    /// Стандартные роли: 1 - Owner, 2 - Admin, 3 - User
    /// </remarks>
    public class Role
    {
        /// <summary>
        /// Уникальный идентификатор роли
        /// </summary>
        public int Id { get; set; } 
        /// <summary>
        /// Название роли
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Коллекция участников проекта с данной ролью
        /// </summary>
        public ICollection<Participant>? Participants { get; set; }
    }
}
