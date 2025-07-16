using Mapster;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Interfaces;
using ProjectManagementSystemBackend.Models.DTO;

namespace ProjectManagementSystemBackend.Services
{
    public class TaskService : ITaskService
    {
        ApplicationContext _context;
        ITaskHistoryService _taskHistoryService;
        TypeAdapterConfig config = new TypeAdapterConfig();
        public TaskService(ApplicationContext context, ITaskHistoryService taskHistoryService) 
        {
            _taskHistoryService = taskHistoryService;
            _context = context;
        }
        public async Task DeleteAsync(int taskId, CancellationToken cancellationToken)
        {
            var task = await _context.Tasks.FindAsync(taskId, cancellationToken);
            if (task is null)
                throw new KeyNotFoundException();
            
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<TaskDTO>> GetAsync(int boardStatusId, CancellationToken cancellationToken)
        {
            var tasks = await _context.Tasks
                .Where(t => t.BoardStatusId == boardStatusId)
                .ProjectToType<TaskDTO>()
                .ToListAsync(cancellationToken);
                
            return tasks;
        }

        public async Task<TaskDTO> PostAsync(TaskDTO task, int userId, CancellationToken cancellationToken)
        {
            var existParticipant = await _context.BoardStatuses
                .Include(bs => bs.BaseBoard)
                .ThenInclude(bb => bb.Project)
                .ThenInclude(bb => bb.Participants)
                .Where(bs => bs.Id == task.BoardStatusId &&
                bs.BaseBoard.Project.Participants.Any(p => p.UserId == userId))
                .FirstOrDefaultAsync(cancellationToken);
            if (existParticipant is null)
                throw new KeyNotFoundException("Participant not found");
            var existResponsiblePerson = await _context.Participants.FindAsync(task.ResponsiblePersonId,cancellationToken);
            if (existResponsiblePerson is null)
                throw new InvalidDataException("Invalid responsible person id");

            Models.Task newTask = task.Adapt<Models.Task>(config.Fork(f => f.ForType<TaskDTO, Models.Task>().Ignore("Id")));
            newTask.CreatorId = existParticipant.BaseBoard.Project.Participants
                .First(p => p.UserId == userId).Id;
            newTask.LastUpdate = DateTime.UtcNow;

            await _context.Tasks.AddAsync(newTask,cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            await _taskHistoryService.CreateAsync(newTask, userId, cancellationToken);

            return newTask.Adapt<TaskDTO>();
        }

        public async Task UpdateAsync(TaskDTO updatedTask, int userId, CancellationToken cancellationToken)
        {
            var existBoardStauts = await _context.BoardStatuses.FindAsync(updatedTask.BoardStatusId, cancellationToken);
            if (existBoardStauts is null)
                throw new InvalidDataException("Invalid board status id");

            var existResponsiblePerson = await _context.Participants.FindAsync(updatedTask.ResponsiblePersonId, cancellationToken);
            if (existResponsiblePerson is null)
                throw new InvalidDataException("Invalid responsible person id");

            var oldTask = await _context.Tasks.AsNoTracking().FirstOrDefaultAsync(t => t.Id == updatedTask.Id, cancellationToken);
            var task = await _context.Tasks.FindAsync(updatedTask.Id, cancellationToken);
            if (task is null)
                throw new KeyNotFoundException("Task not found");
            updatedTask.Adapt(task);
            task.LastUpdate = DateTime.UtcNow;
            task.CreatorId = oldTask.CreatorId;
            await _context.SaveChangesAsync(cancellationToken);

            await _taskHistoryService.UpdateAsync(oldTask, task, userId,cancellationToken);
        }
    }
}
