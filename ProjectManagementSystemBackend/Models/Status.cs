namespace ProjectManagementSystemBackend.Models
{
    /// <summary>
    /// Статус задачи
    /// </summary>
    /// <remarks>
    /// Определяет возможные состояния задач в системе.
    /// Стандартные статусы: Новые, В работе, Проверяются, Готовые.
    /// </remarks>
    public class Status
    {
        /// <summary>
        /// Уникальный идентификатор статуса
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Название статуса
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Коллекция статусов досок, зависящих от данного статуса
        /// </summary>
        public ICollection<BoardStatus>? BoardStatuses  { get; set; }
    }
}
