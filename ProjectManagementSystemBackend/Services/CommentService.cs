using Mapster;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Interfaces;
using ProjectManagementSystemBackend.Models;
using ProjectManagementSystemBackend.Models.DTO;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagementSystemBackend.Services
{
    public class CommentService : ICommentService
    {
        ApplicationContext _context;
        TypeAdapterConfig config = new TypeAdapterConfig();

        public CommentService(ApplicationContext context) 
        {
            _context= context;
        }
        public async Task DeleteAsync(int commentId, CancellationToken cancellationToken)
        {

            var comment = await _context.TaskComments.FindAsync(commentId,cancellationToken);
            if (comment is null)
                throw new KeyNotFoundException();
            _context.TaskComments.Remove(comment);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<CommentDTO?>> GetAsync(int taskId, CancellationToken cancellationToken)
        {
            var comments = await _context.TaskComments
                .AsNoTracking()
                .Where(c => c.TaskId == taskId)
                .ProjectToType<CommentDTO>()
                .ToListAsync(cancellationToken);
            return comments;
        }

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

        public async Task<CommentDTO> UpdateAsync(CommentDTO comment, CancellationToken cancellationToken)
        {
            var updatedComment = await _context.TaskComments.FindAsync(comment.Id, cancellationToken);
            if (updatedComment is null)
                return null;
            updatedComment.Message = comment.Message;
            await _context.SaveChangesAsync(cancellationToken);
            return updatedComment.Adapt<CommentDTO>();
        }
    }
}
