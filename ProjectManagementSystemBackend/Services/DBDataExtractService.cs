using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Interfaces;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;

namespace ProjectManagementSystemBackend.Services
{
    /// <summary>
    /// Класс для извлечения данных из таблиц БД 
    /// </summary>
    public class DBDataExtractService : IDBDataExtractService
    {
        ApplicationContext _context;
        /// <summary>
        /// Конструктор класса для извлечения данных из БД 
        /// </summary>
        /// <param name="context"></param>
        public DBDataExtractService(ApplicationContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Метод для получения данных из всех таблиц
        /// </summary>
        /// <returns>Словарь с данными, ключ - название таблицы, значение - данные таблицы  </returns>
        public async Task<Dictionary<string, IEnumerable<object>>> GetAllTablesDataAsync()
        {
            var result = new Dictionary<string, IEnumerable<object>>();
            var dbSets = GetDbSetsProperties();
            foreach(var dbSet in dbSets)
            {
                var data =await GetTablesDataAsync(dbSet);
                result.Add(dbSet.Name, data);
            }   
            return result;
        }

        private IEnumerable<PropertyInfo> GetDbSetsProperties()
        {
            return _context.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType.IsGenericType && 
                p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>)); 
        }

        private async Task<IEnumerable<Object>> GetTablesDataAsync(PropertyInfo dbSetProperty)
        {
            var dbSet = dbSetProperty.GetValue(_context) as IQueryable<object>;
            return dbSet is null
                ? throw new InvalidDataException($"Invalid dbSetProperty {dbSetProperty.Name}")
                : await dbSet.AsNoTracking().ToListAsync();
        }
    }
}
