namespace ProjectManagementSystemBackend.Models
{
    /// <summary>
    /// Участник проекта, соединяет пользователей системы и проекты
    /// </summary>
    public class Participant
    {
        /// <summary>
        /// Уникальный идентификатор участника
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Код проекта
        /// </summary>
        public int ProjectId { get; set; }
        /// <summary>
        /// Код пользователя
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// Код роли
        /// </summary>
        public int RoleId { get; set; }
        /// <summary>
        /// Объект проекта
        /// </summary>
        public Project? Project { get; set; }
        /// <summary>
        /// Объект пользователя
        /// </summary>
        public User? User { get; set; }
        /// <summary>
        /// Объект роли
        /// </summary>
        public Role? Role { get ; set; }
        /// <summary>
        /// Коллекция историй задач в которых участник изменял данные
        /// </summary>
        public ICollection<TaskHistory>? TaskHistories { get; set; }
        /// <summary>
        /// Коллекция созданных задач участника
        /// </summary>
        public ICollection<Task>? CreatedTasks { get; set; }
        /// <summary>
        /// Коллекция задач, в которых участник выступает в роли ответственного
        /// </summary>
        public ICollection<Task>? ResponsibleTasks { get; set; }
        /// <summary>
        /// Коллекция комментариев к задачам, написанных участником
        /// </summary>
        public ICollection<TaskComment>? TaskComments { get; set; }
    }
}
