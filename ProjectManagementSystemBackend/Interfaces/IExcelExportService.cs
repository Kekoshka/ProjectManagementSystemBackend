using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystemBackend.Models.DTO;

namespace ProjectManagementSystemBackend.Interfaces
{
    /// <summary>
    /// Интерфейс для экспорта данных из БД в Excel формат 
    /// </summary>
    public interface IExcelExportService
    {
        /// <summary>
        /// Метод для экспорта данных из таблиц БД в Excel файл
        /// </summary>
        /// <param name="tablesData">Данные таблиц</param>
        /// <returns>Массив байт с даннными Excel файла</returns>
        Task<FileDTO> ExportAllTablesToExcelAsync();
    }
}
