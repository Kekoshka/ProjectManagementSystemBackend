namespace ProjectManagementSystemBackend.Models
{
    /// <summary>
    /// Основная сущность системы
    /// </summary>
    /// <remarks>
    ///  Содержит задачи, участников и доски проекта 
    ///  </remarks>
    public class Project
    {
        /// <summary>
        /// Уникальный идентификатор проекта
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Название проекта
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Описание проекта
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Приватность проекта
        /// </summary>
        public bool Security {  get; set; }
        /// <summary>
        /// Коллекция базовых досок проекта
        /// </summary>
        public ICollection<BaseBoard>? BaseBoards { get; set; }
        /// <summary>
        /// Коллекция участников проекта
        /// </summary>
        public ICollection<Participant>? Participants { get; set; }
    }
}
