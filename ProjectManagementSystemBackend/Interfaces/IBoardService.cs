using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystemBackend.Models.DTO;

namespace ProjectManagementSystemBackend.Interfaces
{
    public interface IBoardService
    {
        Task<IEnumerable<BaseBoardDTO>> GetBoardsByProjectIdAsync(int projectId, CancellationToken cancellationToken);
        Task<object?> GetByBaseBoardIdAsync(int baseBoardId, CancellationToken cancellationToken);
        Task<CanbanBoardDTO> PostCanbanAsync(CanbanBoardDTO canbanBoard, CancellationToken cancellationToken);
        Task<ScrumBoardDTO> PostScrumAsync(ScrumBoardDTO scrumBoard, CancellationToken cancellationToken);
        Task UpdateCanbanBoardAsync(CanbanBoardDTO canbanBoard, CancellationToken cancellationToken);
        Task UpdateScrumBoardAsync(ScrumBoardDTO ScrumBoard, CancellationToken cancellationToken);
        Task DeleteAsync(int baseBoardId, CancellationToken cancellationToken);
    }
}
