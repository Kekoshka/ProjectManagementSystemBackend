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
                .ToListAsync();
            return comments;
        }

        public async Task<CommentDTO> PostAsync(CommentDTO comment, CancellationToken cancellationToken)
        {
            TaskComment newComment = comment.Adapt<TaskComment>();
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
