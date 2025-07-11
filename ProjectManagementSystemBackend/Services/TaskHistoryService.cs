﻿using Microsoft.EntityFrameworkCore;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Interfaces;
using ProjectManagementSystemBackend.Models;
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
        public async Task CreateAsync(Models.Task task, int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user is null)
                return;
            var responsiblePerson = await _context.Participants.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == task.ResponsiblePersonId);
            string actionString = $"{user.Name} создал(а) новую задачу с названием '{task.Name}', описанием '{task.Description}' и ответственным '{responsiblePerson.User.Name}'";
            
            TaskHistory newTaskHistory = new(DateTime.UtcNow, actionString, userId, task.Id, 1);
            await _context.TaskHistories.AddAsync(newTaskHistory);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Models.Task oldTask,Models.Task newTask, int userId)
        {
            var user = await _context.Users.FindAsync(userId);
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
            await _context.TaskHistories.AddAsync(newTaskHistory);
            await _context.SaveChangesAsync();
        }
    }
}
