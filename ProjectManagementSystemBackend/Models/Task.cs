namespace ProjectManagementSystemBackend.Models
{
    /// <summary>
    /// Задача проекта
    /// </summary>
    public class Task
    {
        /// <summary>
        /// Уникаьлный идентификатор задачи
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Название задачи
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Описание задачи
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Приоритет задачи
        /// </summary>
        public int Priority { get; set; }
        /// <summary>
        /// Дата последнего обновления задачи
        /// </summary>
        public DateTime LastUpdate { get; set; }
        /// <summary>
        /// Крайний срок выполнения задачи
        /// </summary>
        public DateTime TimeLimit { get; set; }
        /// <summary>
        /// Код создателя задачи
        /// </summary>
        public int CreatorId { get; set; }
        /// <summary>
        /// Код ответственного за задачу
        /// </summary>
        public int ResponsiblePersonId { get; set; }
        /// <summary>
        /// Код статуса задачи
        /// </summary>
        public int BoardStatusId { get; set; }
        /// <summary>
        /// Объект статуса доски
        /// </summary>
        public BoardStatus? BoardStatus { get; set; }
        /// <summary>
        /// Объект участника ответственного за задачу
        /// </summary>
        public Participant? ResponsiblePerson { get; set; }
        /// <summary>
        /// Объект создателя задачи
        /// </summary>
        public Participant? Creator { get; set; }
        /// <summary>
        /// Объект комментариев задачи
        /// </summary>
        public ICollection<TaskComment>? TaskComments { get; set; }
        /// <summary>
        /// Стандартный конструктор задачи
        /// </summary>
        public Task() { }
        /// <summary>
        /// Конструктор для создания задачи
        /// </summary>
        /// <param name="name">Название задачи</param>
        /// <param name="description">Описание задачи</param>
        /// <param name="priority">Приоритет задачи</param>
        /// <param name="lastUpdate">Дата последнего обновления</param>
        /// <param name="timeLimit">Крайний срок выполнения задачи</param>
        /// <param name="creatorId">ID создателя задачи</param>
        /// <param name="responsiblePersonId">ID ответственного</param>
        /// <param name="boardStatusId">ID статуса задачи</param>
        public Task(string name, string description, int priority, DateTime lastUpdate, DateTime timeLimit, int creatorId, int responsiblePersonId, int boardStatusId)
        
        {
            Name = name;
            Description = description;
            Priority = priority;
            LastUpdate = lastUpdate;
            TimeLimit = timeLimit;
            CreatorId = creatorId;
            ResponsiblePersonId = responsiblePersonId;
            BoardStatusId = boardStatusId;
        }
    }
}
