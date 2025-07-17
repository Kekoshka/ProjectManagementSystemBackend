using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystemBackend.Models.DTO;

namespace ProjectManagementSystemBackend.Interfaces
{
    public interface IBoardService
    {
        Task<IEnumerable<BaseBoardDTO>> GetBoardsByProjectIdAsync(int projectId, CancellationToken cancellationToken);
        Task<object?> GetByBaseBoardIdAsync(int baseBoardId, CancellationToken cancellationToken);
        Task<KanbanBoardDTO> PostKanbanAsync(KanbanBoardDTO kanbanBoard, CancellationToken cancellationToken);
        Task<ScrumBoardDTO> PostScrumAsync(ScrumBoardDTO scrumBoard, CancellationToken cancellationToken);
        Task UpdateKanbanBoardAsync(KanbanBoardDTO kanbanBoard, CancellationToken cancellationToken);
        Task UpdateScrumBoardAsync(ScrumBoardDTO ScrumBoard, CancellationToken cancellationToken);
        Task DeleteAsync(int baseBoardId, CancellationToken cancellationToken);
    }
}
