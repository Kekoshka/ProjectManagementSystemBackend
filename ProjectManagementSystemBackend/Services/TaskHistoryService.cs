using Mapster;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Interfaces;
using ProjectManagementSystemBackend.Models;
using ProjectManagementSystemBackend.Models.DTO;
using System.Reflection;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagementSystemBackend.Services
{
    /// <summary>
    /// Сервис для управления данными о истории задач
    /// </summary>
    /// <remarks>
    /// Позволяет получать, создавать, изменять и удалять данные о истории задачи
    /// </remarks>
    public class TaskHistoryService : ITaskHistoryService
    {
        ApplicationContext _context;
        /// <summary>
        /// Конструктор сервиса истории задач
        /// </summary>
        /// <param name="context">Контекст базы данных</param>
        public TaskHistoryService(ApplicationContext context) 
        {
            _context = context;
        }

        /// <summary>
        /// Добавить новую запись в историю задачи о создании задачи
        /// </summary>
        /// <param name="task">Данные созданной задачи</param>
        /// <param name="userId">ID пользователя</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <exception cref="KeyNotFoundException">Если пользователь не найден</exception>
        public async Task CreateAsync(Models.Task task, int userId, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FindAsync(userId, cancellationToken);
            if (user is null)
                throw new KeyNotFoundException($"User with {userId} id not found");
            var responsiblePerson = await _context.Participants
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == task.ResponsiblePersonId, cancellationToken);
            string actionString = $"{user.Name} создал(а) новую задачу с названием '{task.Name}'," +
                $" описанием '{task.Description}' и ответственным '{responsiblePerson.User.Name}'";
            
            TaskHistory newTaskHistory = new(DateTime.UtcNow, actionString, userId, task.Id, 1);
            await _context.TaskHistories.AddAsync(newTaskHistory, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Получить историю задачи
        /// </summary>
        /// <param name="taskId">ID задачи</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Список с DTO записей из истории задачи</returns>
        public async Task<IEnumerable<TaskHistoryDTO>> GetAsync(int taskId, CancellationToken cancellationToken)
        {
            var taskHistories = await _context.TaskHistories
                .ProjectToType<TaskHistoryDTO>()
                .Where(th => th.TaskId == taskId)
                .ToListAsync(cancellationToken);
            return taskHistories;
        }

        /// <summary>
        /// Добавить новую запись в историю задачи об обновлении задачи
        /// </summary>
        /// <param name="oldTask">Старая задача</param>
        /// <param name="newTask">Новая задача</param>
        /// <param name="userId">ID пользователя</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <exception cref="KeyNotFoundException">Если пользователь не найден</exception>
        /// <exception cref="InvalidDataException">Если передано null значение старой или новой задачи</exception>
        public async Task UpdateAsync(Models.Task oldTask,Models.Task newTask, int userId, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FindAsync(userId, cancellationToken);
            if (user is null)
                throw new KeyNotFoundException($"User with {userId} id not found");
            string actionString = $"{user.Name} внес изменения: ";

            if (oldTask is null || newTask is null)
                throw new InvalidDataException("Old or new task is null");

            PropertyInfo[] properties = typeof(Models.Task).GetProperties();
            foreach(PropertyInfo property in properties)
            {
                object oldValue = property.GetValue(oldTask);
                object newValue = property.GetValue(newTask);
                if (newValue == default || oldValue == default)
                    continue;
                if (!Equals(oldValue, newValue))
                    actionString += $"'{property.Name}' изменено с '{oldValue}' на '{newValue}' ";
            }

            TaskHistory newTaskHistory = new(DateTime.UtcNow, actionString, userId, oldTask.Id, 2);
            await _context.TaskHistories.AddAsync(newTaskHistory, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
