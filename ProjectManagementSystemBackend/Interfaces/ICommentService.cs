using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystemBackend.Models;
using ProjectManagementSystemBackend.Models.DTO;

namespace ProjectManagementSystemBackend.Interfaces
{
    public interface ICommentService
    {
        Task<IEnumerable<TaskComment?>> GetAsync(int taskId, CancellationToken cancellationToken);
        Task<IEnumerable<TaskComment?>> PostAsync(CommentDTO comment, CancellationToken cancellationToken);
        Task<IEnumerable<TaskComment?>> UpdateAsync(CommentDTO comment, CancellationToken cancellationToken);
        Task<IEnumerable<TaskComment?>> DeleteAsync(int commentId, CancellationToken cancellationToken);


    }
}
