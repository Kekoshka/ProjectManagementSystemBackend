namespace ProjectManagementSystemBackend.Models
{
    /// <summary>
    /// Используется в качестве основы для Kanban и Scrum досок
    /// </summary>
    /// <remarks>
    /// Выступает в качестве родительской сущности для Kanban и Scrum досок,
    /// хранит в себе общие свойства и связи
    /// </remarks>
    public class BaseBoard
    {
        /// <summary>
        /// Уникальный идентификатор базовой доски
        /// </summary>
        public int Id {  get; set; }
        /// <summary>
        /// Название базовой доски
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Описание базовой доски
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Код проекта, которому пренадлежит базовая доска 
        /// </summary>
        public int? ProjectId { get; set; }
        /// <summary>
        /// Проект, которому пренадлежит базовая доска
        /// </summary>
        public Project? Project { get; set; }
        /// <summary>
        /// Коллекция статусов базовой доски
        /// </summary>
        public ICollection<BoardStatus>? BoardStatuses { get; set; }
    }
}
