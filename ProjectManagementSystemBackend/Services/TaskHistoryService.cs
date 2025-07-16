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
    public class TaskHistoryService : ITaskHistoryService
    {
        ApplicationContext _context;
        public TaskHistoryService(ApplicationContext context) 
        {
            _context = context;
        }
        public async Task CreateAsync(Models.Task task, int userId, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FindAsync(userId, cancellationToken);
            if (user is null)
                return;
            var responsiblePerson = await _context.Participants
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == task.ResponsiblePersonId, cancellationToken);
            string actionString = $"{user.Name} создал(а) новую задачу с названием '{task.Name}'," +
                $" описанием '{task.Description}' и ответственным '{responsiblePerson.User.Name}'";
            
            TaskHistory newTaskHistory = new(DateTime.UtcNow, actionString, userId, task.Id, 1);
            await _context.TaskHistories.AddAsync(newTaskHistory, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<TaskHistoryDTO>> GetAsync(int taskId, CancellationToken cancellationToken)
        {

            var taskHistories = await _context.TaskHistories
                .ProjectToType<TaskHistoryDTO>()
                .Where(th => th.TaskId == taskId)
                .ToListAsync(cancellationToken);
            return taskHistories;
        }

        public async Task UpdateAsync(Models.Task oldTask,Models.Task newTask, int userId, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FindAsync(userId, cancellationToken);
            if (user is null)
                return;
            string actionString = $"{user.Name} внес изменения: ";

            if (oldTask is null || newTask is null)
                return;

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
