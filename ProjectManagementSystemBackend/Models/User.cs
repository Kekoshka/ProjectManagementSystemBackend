namespace ProjectManagementSystemBackend.Models
{
    /// <summary>
    /// Пользователь системы
    /// </summary>
    /// <remarks>
    /// Учетная запись для авторизации в системе.
    /// Содержит основные данные пользователя и связи с проектами через Participant.
    /// </remarks>
    public class User
    {
        /// <summary>
        /// Уникальный идентификатор пользователя
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Логин пользователя
        /// </summary>
        public string Login { get; set; }
        /// <summary>
        /// Пароль пользователя в зашифрованном виде
        /// </summary>
        public string Password { get; set; }    
        /// <summary>
        /// Коллекция участников проектов к которым относится данный пользователь 
        /// </summary>
        public ICollection<Participant>? Participants { get; set; }
    }
}
