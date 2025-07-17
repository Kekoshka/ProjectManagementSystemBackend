using System.ComponentModel.DataAnnotations;

namespace ProjectManagementSystemBackend.Models
{
    /// <summary>
    /// Тип действия в истории изменений задачи
    /// </summary>
    /// <remarks>
    /// Определяет вид действия, которое было выполнено над задачей:
    /// </remarks>
    public class ActionType
    {
        /// <summary>
        /// Уникальный идентификатор типа действия
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Название типа действия
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Коллекция записей истории задач, связанных с данным типом действия
        /// </summary>
        public ICollection<TaskHistory>? TaskHistories { get; set; }
    }
}
