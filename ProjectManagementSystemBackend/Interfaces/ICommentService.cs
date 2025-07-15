using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystemBackend.Models;
using ProjectManagementSystemBackend.Models.DTO;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagementSystemBackend.Interfaces
{
    public interface ICommentService
    {
        Task<IEnumerable<CommentDTO?>> GetAsync(int taskId, CancellationToken cancellationToken);
        Task<CommentDTO> PostAsync(CommentDTO comment, CancellationToken cancellationToken);
        Task<CommentDTO> UpdateAsync(CommentDTO comment, CancellationToken cancellationToken);
        Task DeleteAsync(int commentId, CancellationToken cancellationToken);
    }
}
