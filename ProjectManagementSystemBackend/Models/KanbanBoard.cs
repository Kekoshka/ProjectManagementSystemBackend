namespace ProjectManagementSystemBackend.Models
{
    /// <summary>
    /// Канбан доска, дочерняя сущность базовой доски
    /// </summary>
    /// <remarks>
    /// Хранит в себе свойства присущие Kanban доскам
    /// </remarks>
    public class KanbanBoard
    {
        /// <summary>
        /// Уникальный идентификатор Kanban доски
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Лимит задач доски
        /// </summary>
        public int TaskLimit { get; set; }
        /// <summary>
        /// Код базовой доски
        /// </summary>
        public int BaseBoardId { get; set; }
        /// <summary>
        /// Объект базовой доски
        /// </summary>
        public BaseBoard? BaseBoard { get; set; }
    }
}
