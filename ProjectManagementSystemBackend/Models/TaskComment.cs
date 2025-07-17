namespace ProjectManagementSystemBackend.Models
{
    /// <summary>
    /// Комментарий к задаче
    /// </summary>
    /// <remarks>
    /// Позволяет участникам проекта оставлять заметки и обсуждения по конкретной задаче.
    /// Каждый комментарий привязан к задаче и автору (участнику проекта).
    /// </remarks>
    public class TaskComment
    {
        /// <summary>
        /// Уникальный идентификатор комментария задачи
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Сообщение
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Код участника, оставившего сообщение
        /// </summary>
        public int ParticipantId { get; set; }
        /// <summary>
        /// Код задачи для которой предназначено сообщение
        /// </summary>
        public int TaskId {  get; set; }
        /// <summary>
        /// Объект участника, оставившего сообщение
        /// </summary>
        public Participant? Participant { get; set; }
        /// <summary>
        /// Объект задачи для которой предназначено сообщение
        /// </summary>
        public Task? Task { get; set; }
    }
}
