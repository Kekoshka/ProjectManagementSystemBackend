using Mapster;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Interfaces;
using ProjectManagementSystemBackend.Models.DTO;

namespace ProjectManagementSystemBackend.Services
{
    /// <summary>
    /// Сервис для работы с задачами проекта
    /// </summary>
    /// <remarks>
    /// Предоставляет функционал для создания, получения, обновления и удаления задач.
    /// Автоматически ведет историю изменений задач через ITaskHistoryService.
    /// </remarks>
    public class TaskService : ITaskService
    {
        ApplicationContext _context;
        ITaskHistoryService _taskHistoryService;
        TypeAdapterConfig config = new TypeAdapterConfig();

        /// <summary>
        /// Конструктор сервиса задач
        /// </summary>
        /// <param name="context">Контекст базы данных</param>
        /// <param name="taskHistoryService">Сервис истории задач</param>
        public TaskService(ApplicationContext context, ITaskHistoryService taskHistoryService) 
        {
            _taskHistoryService = taskHistoryService;
            _context = context;
        }

        /// <summary>
        /// Удалить задачу
        /// </summary>
        /// <param name="taskId">ID задачи</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Task</returns>
        /// <exception cref="KeyNotFoundException">Если задача не найдена</exception>
        public async Task DeleteAsync(int taskId, CancellationToken cancellationToken)
        {
            var task = await _context.Tasks.FindAsync(taskId, cancellationToken);
            if (task is null)
                throw new KeyNotFoundException();
            
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Получить задачи по статусу доски
        /// </summary>
        /// <param name="boardStatusId">ID статуса доски</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Список DTO задач</returns>
        public async Task<IEnumerable<TaskDTO>> GetAsync(int boardStatusId, CancellationToken cancellationToken)
        {
            var tasks = await _context.Tasks
                .Where(t => t.BoardStatusId == boardStatusId)
                .ProjectToType<TaskDTO>()
                .ToListAsync(cancellationToken);
                
            return tasks;
        }

        /// <summary>
        /// Создать новую задачу
        /// </summary>
        /// <param name="task">DTO с данными задачи</param>
        /// <param name="userId">ID пользователя-создателя</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>DTO новой задачи</returns>
        /// <exception cref="KeyNotFoundException">
        /// Если участник проекта не найден
        /// </exception>
        /// <exception cref="InvalidDataException">
        /// Если указан неверный ответственный
        /// </exception>
        /// <remarks>
        /// При создании задачи:
        /// 1. Проверяется валидность участника проекта
        /// 2. Проверяется валидность ответственного
        /// 3. Автоматически устанавливается создатель и дата создания
        /// 4. Создается запись в истории задач
        /// </remarks>
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

            try
            {
                await _taskHistoryService.CreateAsync(newTask, userId, cancellationToken);
            }
            catch (KeyNotFoundException ex) { throw new KeyNotFoundException($"Exception in task history service: '{ex.Message}'"); }
            catch (Exception ex) { throw new Exception(ex.Message); }
            return newTask.Adapt<TaskDTO>();
        }

        /// <summary>
        /// Обновить существующую задачу
        /// </summary>
        /// <param name="updatedTask">DTO задачи с обновленными данными</param>
        /// <param name="userId">ID пользователя</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Task</returns>
        /// <exception cref="InvalidDataException">
        /// Если указан неверный статус доски или ответственный
        /// </exception>
        /// <exception cref="KeyNotFoundException">Если задача не найдена</exception>
        /// <remarks>
        /// При обновлении задачи:
        /// 1. Проверяется валидность статуса доски и ответственного
        /// 2. Сохраняются оригинальные данные создателя
        /// 3. Обновляется дата последнего изменения
        /// 4. Создается запись в истории задач
        /// </remarks>
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

            try
            {
                await _taskHistoryService.UpdateAsync(oldTask,task, userId, cancellationToken);
            }
            catch (InvalidDataException ex) { throw new InvalidDataException($"Exception in task history service: '{ex.Message}'"); }
            catch (KeyNotFoundException ex) { throw new KeyNotFoundException($"Exception in task history service: '{ex.Message}'"); }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
