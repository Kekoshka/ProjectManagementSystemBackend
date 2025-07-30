namespace ProjectManagementSystemBackend.Interfaces
{
    /// <summary>
    /// Интерфейс для получения данных из таблиц базы данных
    /// </summary>
    public interface IDBDataExtractService
    {
        /// <summary>
        /// Метод для получения данных из всех таблиц
        /// </summary>
        /// <returns>Словарь с данными, ключ - название таблицы, значение - данные таблицы  </returns>
        Task<Dictionary<string, IEnumerable<object>>> GetAllTablesDataAsync();
    }
}
