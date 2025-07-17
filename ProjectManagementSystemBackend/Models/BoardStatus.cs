namespace ProjectManagementSystemBackend.Models
{
    /// <summary>
    /// Связывает базовую доску и статус, исключая связь многие ко многим
    /// </summary>
    /// <remarks>
    /// Определяет статусы в досках проекта
    /// Позволяет задать уникальные наборы статусов для разных досок проекта
    /// </remarks>  
    public class BoardStatus
    {
        /// <summary>
        /// Уникальный идентификатор статуса доски
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Код базовой доски
        /// </summary>
        public int BaseBoardId{ get; set; }
        /// <summary>
        /// Код статуса
        /// </summary>
        public int StatusId { get; set; }
        /// <summary>
        /// Объект базовой доски
        /// </summary>
        public BaseBoard? BaseBoard { get; set; }
        /// <summary>
        /// Объект статуса
        /// </summary>
        public Status? Status { get; set; }
        /// <summary>
        /// Коллекция задач, пренадлежащих статусу
        /// </summary>
        public ICollection<Task>? Tasks { get; set; }
    }
}
