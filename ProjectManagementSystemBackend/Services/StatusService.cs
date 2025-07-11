using Microsoft.EntityFrameworkCore;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Interfaces;
using ProjectManagementSystemBackend.Models;
using ProjectManagementSystemBackend.Models.DTO;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagementSystemBackend.Services
{
    public class StatusService : IStatusService
    {
        ApplicationContext _context;
        public StatusService(ApplicationContext context)
        {
            _context = context;
        }
        public Task DeleteAsync(int boardStatusId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<StatusDTO>> GetAsync(int baseBoardId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<StatusDTO> PostAsync(StatusDTO status, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(StatusDTO status, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        public async Task CreateBaseStatusesAsync(int baseBoardId, CancellationToken cancellationToken)
        {
            List<BoardStatus> newBoardStatuses = [
                new BoardStatus{ BaseBoardId = baseBoardId, StatusId = 1 },
                new BoardStatus{ BaseBoardId = baseBoardId, StatusId = 2 },
                new BoardStatus{ BaseBoardId = baseBoardId, StatusId = 3 },
                new BoardStatus{ BaseBoardId = baseBoardId, StatusId = 4 }];
            await _context.BoardStatuses.AddRangeAsync(newBoardStatuses, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
