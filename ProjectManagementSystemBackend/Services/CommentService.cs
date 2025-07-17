using Mapster;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Interfaces;
using ProjectManagementSystemBackend.Models;
using ProjectManagementSystemBackend.Models.DTO;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagementSystemBackend.Services
{
    /// <summary>
    /// Сервис для работы с комментариями
    /// </summary>
    public class CommentService : ICommentService
    {
        ApplicationContext _context;
        TypeAdapterConfig config = new TypeAdapterConfig();
        /// <summary>
        /// Конструктор сервиса комментариев
        /// </summary>
        /// <param name="context">Контекст базы данных</param>
        public CommentService(ApplicationContext context) 
        {
            _context= context;
        }
        /// <summary>
        /// Удалить комментарий
        /// </summary>
        /// <param name="commentId">ID комментария</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Статус операции</returns>
        /// <exception cref="KeyNotFoundException">Если комментарий не найден</exception>
        public async Task DeleteAsync(int commentId, CancellationToken cancellationToken)
        {
            var comment = await _context.TaskComments.FindAsync(commentId,cancellationToken);
            if (comment is null)
                throw new KeyNotFoundException();
            _context.TaskComments.Remove(comment);
            await _context.SaveChangesAsync(cancellationToken);
        }
        /// <summary>
        /// Получить комментарии задачи
        /// </summary>
        /// <param name="taskId">ID задачи</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Список DTO комментариев задачи</returns>
        public async Task<IEnumerable<CommentDTO?>> GetAsync(int taskId, CancellationToken cancellationToken)
        {
            var comments = await _context.TaskComments
                .AsNoTracking()
                .Where(c => c.TaskId == taskId)
                .ProjectToType<CommentDTO>()
                .ToListAsync(cancellationToken);
            return comments;
        }
        /// <summary>
        /// Опубликовать комментарий задачи
        /// </summary>
        /// <param name="comment">DTO с данными нового комментария</param>
        /// <param name="userId">ID пользователя</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>DTO созданного комментария</returns>
        public async Task<CommentDTO> PostAsync(CommentDTO comment, int userId, CancellationToken cancellationToken)
        {
            TaskComment newComment = comment.Adapt<TaskComment>(config.Fork(f => f.ForType<CommentDTO,TaskComment>().Ignore("Id")));

            var task = await _context.Tasks
                .Include(t => t.BoardStatus)
                .ThenInclude(bs => bs.BaseBoard)
                .ThenInclude(bb => bb.Project)
                .ThenInclude(p => p.Participants)
                .Where(t => t.Id == comment.TaskId &&
                t.BoardStatus.BaseBoard.Project.Participants.Any(p => p.UserId == userId))
                .FirstOrDefaultAsync(cancellationToken);
            newComment.ParticipantId = task.BoardStatus.BaseBoard.Project.Participants.First(p => p.UserId == userId).Id;
            await _context.TaskComments.AddAsync(newComment, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return newComment.Adapt<CommentDTO>();
        }

        /// <summary>
        /// Обновить комментарий
        /// </summary>
        /// <param name="comment">DTO с данными обновленного комментария</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <exception cref="KeyNotFoundException">Комментарий с заданным ID не найден</exception>
        public async Task UpdateAsync(CommentDTO comment, CancellationToken cancellationToken)
        {
            var updatedComment = await _context.TaskComments.FindAsync(comment.Id, cancellationToken);
            if (updatedComment is null)
                throw new KeyNotFoundException($"Not found comment with {comment.Id} id ");
            updatedComment.Message = comment.Message;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
