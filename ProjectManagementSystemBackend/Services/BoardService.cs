using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Interfaces;
using ProjectManagementSystemBackend.Models;
using ProjectManagementSystemBackend.Models.DTO;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagementSystemBackend.Services
{
    public class BoardService : IBoardService
    {
        ApplicationContext _context;
        IStatusService _statusService;
        public BoardService(ApplicationContext context, IStatusService statusService) 
        {
            _context = context;
        }

        
        public async Task<IEnumerable<BaseBoardDTO>> GetBoardsByProjectIdAsync(int projectId, CancellationToken cancellationToken)
        {
            var boards = await _context.BaseBoards
                .Where(b => b.ProjectId == projectId)
                .ProjectToType<BaseBoardDTO>()
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            return boards;
        }

        public async Task<object?> GetByBaseBoardIdAsync(int baseBoardId, CancellationToken cancellationToken)
        {
            var scrumBoard = await _context.ScrumBoards
                .AsNoTracking()
                .ProjectToType<ScrumBoardDTO>()
                .FirstOrDefaultAsync(sb => sb.BaseBoardId == baseBoardId, cancellationToken);
            if (scrumBoard is not null)
                return scrumBoard;

            var canbanBoard = await _context.CanbanBoards
                .AsNoTracking()
                .ProjectToType<CanbanBoardDTO>()
                .FirstOrDefaultAsync(cb => cb.BaseBoardId == baseBoardId, cancellationToken);
            return canbanBoard is null ? null : canbanBoard;
        }

        public async Task<CanbanBoardDTO> PostCanbanAsync(CanbanBoardDTO canbanBoard, CancellationToken cancellationToken)
        {
            BaseBoard newBaseBoard = canbanBoard.BaseBoard.Adapt<BaseBoard>(); //подумать как не мапить id
            await _context.BaseBoards.AddAsync(newBaseBoard, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            CanbanBoard newCanbanBoard = new()
            {
                BaseBoardId = newBaseBoard.Id,
                TaskLimit = canbanBoard.TaskLimit,
                BaseBoard = newBaseBoard//Проверить будет ли подсасываться Id если 
            }; 
            await _context.CanbanBoards.AddAsync(newCanbanBoard,cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            await _statusService.CreateBaseStatusesAsync(newBaseBoard.Id, cancellationToken);

            return newCanbanBoard.Adapt<CanbanBoardDTO>();
        }

        public async Task<ScrumBoardDTO> PostScrumAsync(ScrumBoardDTO scrumBoard, CancellationToken cancellationToken)
        {
            BaseBoard newBaseBoard = scrumBoard.BaseBoard.Adapt<BaseBoard>();
            await _context.BaseBoards.AddAsync(newBaseBoard, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            ScrumBoard newScrumBoard = new()
            {
                BaseBoardId = newBaseBoard.Id,
                TimeLimit = scrumBoard.TimeLimit,
                BaseBoard = newBaseBoard
            };
            await _context.ScrumBoards.AddAsync(newScrumBoard, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return newScrumBoard.Adapt<ScrumBoardDTO>();
        }

        public async Task UpdateCanbanBoardAsync(CanbanBoardDTO newCanbanBoard, CancellationToken cancellationToken)
        {
            var canbanBoard = await _context.CanbanBoards.FindAsync(newCanbanBoard.Id);
            if (canbanBoard is null)
                throw new KeyNotFoundException($"Not found canban board with {newCanbanBoard.Id} id");
            if (canbanBoard.BaseBoardId != newCanbanBoard.BaseBoardId)
                throw new InvalidOperationException ($"CanbanBoard doesnt have BaseBoard with {newCanbanBoard.BaseBoardId} id");
            canbanBoard.TaskLimit = newCanbanBoard.TaskLimit;

            var baseBoard = await _context.BaseBoards.FindAsync(newCanbanBoard.BaseBoardId);
            if (baseBoard is null)
                throw new KeyNotFoundException($"Not found base board with {newCanbanBoard.BaseBoardId} id");
            baseBoard.Name = newCanbanBoard.BaseBoard.Name;
            baseBoard.Description = newCanbanBoard.BaseBoard.Description;
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateScrumBoardAsync(ScrumBoardDTO newScrumBoard, CancellationToken cancellationToken)
        {
            var scrumBoard = await _context.ScrumBoards.FindAsync(newScrumBoard.Id);
            if (scrumBoard is null)
                throw new KeyNotFoundException($"Not found scrum board with {newScrumBoard.Id} id");
            if (scrumBoard.BaseBoardId != newScrumBoard.BaseBoardId)
                throw new InvalidOperationException($"CanbanBoard doesnt have BaseBoard with {newScrumBoard.BaseBoardId} id");
            scrumBoard.TimeLimit = newScrumBoard.TimeLimit;

            var baseBoard = await _context.BaseBoards.FindAsync(newScrumBoard.BaseBoardId);
            if (baseBoard is null)
            throw new KeyNotFoundException($"Not found base board with {newScrumBoard.BaseBoard.Id} id");
            baseBoard.Name = newScrumBoard.BaseBoard.Name;
            baseBoard.Description = newScrumBoard.BaseBoard.Description;
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int baseBoardId, CancellationToken cancellationToken)
        {
            var baseBoard = await _context.BaseBoards.FindAsync(baseBoardId);
            if (baseBoard is null)
                throw new KeyNotFoundException("Not found base board");

            var scrumBoard = await _context.ScrumBoards.FirstOrDefaultAsync(sb => sb.BaseBoardId == baseBoardId);
            var canbanBoard = await _context.CanbanBoards.FirstOrDefaultAsync(cb => cb.BaseBoardId == baseBoardId);

            if (scrumBoard is null && canbanBoard is null)
                throw new KeyNotFoundException("Not found scrum and canban boards");

            _context.BaseBoards.Remove(baseBoard);
            if (scrumBoard is null)
                _context.CanbanBoards.Remove(canbanBoard);
            _context.ScrumBoards.Remove(scrumBoard);
            await _context.SaveChangesAsync();
        }
     
    }
}
