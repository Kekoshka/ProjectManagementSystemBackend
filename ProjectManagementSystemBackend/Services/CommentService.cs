using ProjectManagementSystemBackend.Interfaces;
using ProjectManagementSystemBackend.Models;
using ProjectManagementSystemBackend.Models.DTO;

namespace ProjectManagementSystemBackend.Services
{
    public class CommentService : ICommentService
    {
        public Task<IEnumerable<TaskComment?>> DeleteAsync(int commentId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TaskComment?>> GetAsync(int taskId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TaskComment?>> PostAsync(CommentDTO comment, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TaskComment?>> UpdateAsync(CommentDTO comment, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
