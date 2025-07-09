using ProjectManagementSystemBackend.Interfaces;
using ProjectManagementSystemBackend.Models.DTO;

namespace ProjectManagementSystemBackend.Services
{
    public class BoardService : IBoardService
    {
        public Task DeleteAsync(int baseBoardId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<BaseBoardDTO>> GetBoardsByProjectIdAsync(int projectId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<object?> GetByBaseBoardIdAsync(int baseBoardId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<CanbanBoardDTO> PostCanbanAsync(CanbanBoardDTO canbanBoard, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ScrumBoardDTO> PostScrumAsync(ScrumBoardDTO scrumBoard, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdateCanbanBoardAsync(CanbanBoardDTO canbanBoard, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdateScrumBoardAsync(ScrumBoardDTO ScrumBoard, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
