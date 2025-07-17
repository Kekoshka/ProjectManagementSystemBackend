namespace ProjectManagementSystemBackend.Models
{
    /// <summary>
    /// История изменений задачи
    /// </summary>
    /// <remarks>
    /// Фиксирует все значимые изменения задач (создание, редактирование) 
    /// </remarks>
    public class TaskHistory
    {
        /// <summary>
        /// Уникальный идентификатор запичи в истории задачи
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Дата создания записи в истории задачи
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// Описание действия, выполненного над задачей
        /// </summary>
        public string Action { get; set; }
        /// <summary>
        /// Код пользователя, выполнившего действие
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// Код задачи над которой было выполнено действие
        /// </summary>
        public int TaskId { get; set; }
        /// <summary>
        /// Код типа действия
        /// </summary>
        public int ActionTypeId { get; set; }
        /// <summary>
        /// Объект пользователя, совершившего действие над задачей
        /// </summary>
        public User? User { get; set; }
        /// <summary>
        /// Объект задачи над которой было выполнено действие
        /// </summary>
        public Task? Task { get; set; }
        /// <summary>
        /// Объект типа действия
        /// </summary>
        public ActionType? ActionType { get; set; }
        /// <summary>
        /// Стандартный конструктор истории задачи
        /// </summary>
        public TaskHistory() { }
        /// <summary>
        /// конструктор для создания записи в истории задачи
        /// </summary>
        /// <param name="date">Дата и время изменения</param>
        /// <param name="action">Описание действия</param>
        /// <param name="userId">ID пользователя</param>
        /// <param name="taskId">ID задачи</param>
        /// <param name="actionTypeId">ID типа действия</param>
        public TaskHistory(DateTime date, string action, int userId, int taskId, int actionTypeId)
        {
            Date = date;
            Action = action;
            UserId = userId;
            TaskId = taskId;
            ActionTypeId = actionTypeId;
        }
    }
}
