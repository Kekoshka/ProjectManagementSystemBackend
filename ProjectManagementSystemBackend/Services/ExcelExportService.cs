using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Interfaces;
using ProjectManagementSystemBackend.Models.DTO;
using ProjectManagementSystemBackend.Models.Options;

namespace ProjectManagementSystemBackend.Services
{
    public class ExcelExportService : IExcelExportService
    {
        ApplicationContext _context;
        IDBDataExtractService _dataExtractService;
        ContentTypesOptions _contentTypes;
        public ExcelExportService(ApplicationContext context, IDBDataExtractService dbDataExtractService, IOptions<ContentTypesOptions> contentTypes)
        {
            _context = context;
            _dataExtractService = dbDataExtractService;
            _contentTypes = contentTypes.Value;
        }
        /// <summary>
        /// Метод для экспорта всех таблиц из базы данных в Excel
        /// </summary>
        /// <param name="tablesData">Словарь с данными таблиц из бд</param>
        /// <returns>Массив байт с данными Excel файла</returns>
        public async Task<FileDTO> ExportAllTablesToExcelAsync()
        {
            var tablesData = await _dataExtractService.GetAllTablesDataAsync();
            return new FileDTO
            {
                FileContent = GenerateExcel(tablesData),
                ContentType = _contentTypes.Excel,
                FileName = $"DatabaseExport_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx"
            };
        }
        /// <summary>
        /// Метод для экспорта данных из таблиц БД в Excel файл
        /// </summary>
        /// <param name="tablesData">Данные таблиц</param>
        /// <returns>готовый Excel файл с данными всех таблиц из БД</returns>
        private byte[] GenerateExcel(Dictionary<string,IEnumerable<object>> tablesData)
        {
            using var workbook = new XLWorkbook();
            foreach (var (tableName, data) in tablesData)
                AddWorksheet(workbook, tableName, data);
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private void AddWorksheet(XLWorkbook workbook, string tableName, IEnumerable<object> data)
        {
            int headersRow = 1;
            int dataRow = 2;
            
            if (!data.Any()) return;

            var worksheet = workbook.Worksheets.Add(tableName);
            var properties = data
                .First()
                .GetType()
                .GetProperties()
                .Where(p => IsSimpleType(p.PropertyType))
                .ToList();

            for (int i = 0; i < properties.Count(); i++)
                worksheet.Cell(headersRow, i + 1).Value = properties[i].Name;

            foreach (var item in data)
            {
                for (int i = 0; i < properties.Count; i++)
                    worksheet.Cell(dataRow, i + 1).Value = properties[i].GetValue(item)?.ToString();
                dataRow++;
            }
        }
        private bool IsSimpleType(Type type)
        {
            if (type is null) return false;

            type = Nullable.GetUnderlyingType(type) ?? type;

            if (type.IsPrimitive || type.IsEnum) return true;

            if (type == typeof(string) ||
                type == typeof(decimal) ||
                type == typeof(DateTime) ||
                type == typeof(TimeSpan))
                return true;

            return false;
        }
    }
}
