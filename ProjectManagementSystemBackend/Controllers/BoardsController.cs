using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Models;
using ProjectManagementSystemBackend.Models.DTO;
using System.Security.Claims;

namespace ProjectManagementSystemBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BoardsController : ControllerBase
    {
        ApplicationContext _context;
        int? userId;
        int _userId => userId ??= Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        public BoardsController(ApplicationContext context)
        {
            _context = context;
        }
        [HttpGet("getBaseBoardsByProjectId")]
        public async Task<IActionResult> GetBoardsByProjectId(int projectId)
        {
            var availableProject = await _context.Projects
               .Include(p => p.Participants)
               .AsNoTracking()
               .FirstOrDefaultAsync(p => p.Id == projectId &&
               (p.Security == false ||
               p.Participants.Any(p => p.UserId == _userId)));
            if (availableProject is null)
                return Unauthorized("You havent access to this action");

            var boards = await _context.BaseBoards
                .Where(b => b.ProjectId == projectId)
                .AsNoTracking()
                .ToListAsync();

            return boards is null ? NotFound() : Ok(boards);
        }
        [HttpGet("getBoardByBaseBoardId")]
        public async Task<IActionResult> GetByBaseBoardId(int baseBoardId)
        {
            var availableBoard = await _context.BaseBoards
                .Include(bb => bb.Project)
                .ThenInclude(p => p.Participants)
                .AsNoTracking()
                .FirstOrDefaultAsync(bb => bb.Project.Security == false ||
                bb.Project.Participants.Any(p => p.UserId == _userId));
            if (availableBoard is null)
                return Unauthorized("You havent access to this action");

            var scrumBoard = await _context.ScrumBoards
                .AsNoTracking()
                .FirstOrDefaultAsync(sb => sb.BaseBoardId == baseBoardId);
            if (scrumBoard is not null)
                return Ok(scrumBoard);

            var canbanBoard = await _context.CanbanBoards
                .AsNoTracking()
                .FirstOrDefaultAsync(cb => cb.BaseBoardId == baseBoardId);
            return canbanBoard is null ? NotFound() : Ok(canbanBoard);
        }
        [HttpPost("postCanbanBoard")]
        public async Task<IActionResult> PostCanban(CanbanBoardDTO canbanBoard)
        {
            if (canbanBoard.BaseBoard is null)
                return BadRequest();

            var availableProject = await _context.Projects
               .Include(p => p.Participants)
               .AsNoTracking()
               .FirstOrDefaultAsync(p => p.Id == canbanBoard.BaseBoard.ProjectId &&
               p.Participants.Any(p => p.UserId == _userId && new[] { 1, 2 }.Contains(p.RoleId)));
            if (availableProject is null)
                return Unauthorized("You havent access to this action");

            BaseBoard newBaseBoard = new()
            {
                Name = canbanBoard.BaseBoard.Name,
                Description = canbanBoard.BaseBoard.Description,
                ProjectId = canbanBoard.BaseBoard.ProjectId
            };
            await _context.BaseBoards.AddAsync(newBaseBoard);
            await _context.SaveChangesAsync();

            CanbanBoard newCanbanBoard = new()
            {
                BaseBoardId = newBaseBoard.Id,
                TaskLimit = canbanBoard.TaskLimit
            };
            await _context.CanbanBoards.AddAsync(newCanbanBoard);

            List<BoardStatus> newBoardStatuses = [
                new BoardStatus{ BaseBoardId = newBaseBoard.Id, StatusId = 1 },
                new BoardStatus{ BaseBoardId = newBaseBoard.Id, StatusId = 2 },
                new BoardStatus{ BaseBoardId = newBaseBoard.Id, StatusId = 3 },
                new BoardStatus{ BaseBoardId = newBaseBoard.Id, StatusId = 4 }];
            await _context.BoardStatuses.AddRangeAsync(newBoardStatuses);

            await _context.SaveChangesAsync();

            newCanbanBoard.BaseBoard = newBaseBoard;
            newCanbanBoard.BaseBoard.BoardStatuses = null;
            return Ok(newCanbanBoard);
        }
        [HttpPost("postScrumBoard")]
        public async Task<IActionResult> PostScrum(ScrumBoardDTO scrumBoard)
        {
            if (scrumBoard.BaseBoard is null)
                return BadRequest();

            var availableProject = await _context.Projects
               .Include(p => p.Participants)
               .AsNoTracking()
               .FirstOrDefaultAsync(p => p.Id == scrumBoard.BaseBoard.ProjectId &&
               p.Participants.Any(p => p.UserId == _userId && new[] { 1, 2 }.Contains(p.RoleId)));
            if (availableProject is null)
                return Unauthorized("You havent access to this action");

            BaseBoard newBaseBoard = new()
            {
                Name = scrumBoard.BaseBoard.Name,
                Description = scrumBoard.BaseBoard.Description,
                ProjectId = scrumBoard.BaseBoard.ProjectId
            };
            await _context.BaseBoards.AddAsync(newBaseBoard);
            await _context.SaveChangesAsync();

            ScrumBoard newScrumBoard = new()
            {
                BaseBoardId = newBaseBoard.Id,
                TimeLimit = scrumBoard.TimeLimit
            };
            await _context.ScrumBoards.AddAsync(newScrumBoard);

            List<BoardStatus> newBoardStatuses = [
                new BoardStatus{ BaseBoardId = newBaseBoard.Id, StatusId = 1 },
                new BoardStatus{ BaseBoardId = newBaseBoard.Id, StatusId = 2 },
                new BoardStatus{ BaseBoardId = newBaseBoard.Id, StatusId = 3 },
                new BoardStatus{ BaseBoardId = newBaseBoard.Id, StatusId = 4 }];
            await _context.BoardStatuses.AddRangeAsync(newBoardStatuses);
            await _context.SaveChangesAsync();

            newScrumBoard.BaseBoard = newBaseBoard;
            newScrumBoard.BaseBoard.BoardStatuses = null;
            return Ok(newScrumBoard);
        }

        [HttpPut("updateCanbanBoard")]
        public async Task<IActionResult> UpdateCanbanBoard(CanbanBoardDTO newCanbanBoard)
        {
            var availableProject = await _context.Projects
                .Include(p => p.Participants)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == newCanbanBoard.BaseBoard.ProjectId && 
                    p.Participants.Any(p => p.UserId == _userId && new[] { 1, 2 }.Contains(p.RoleId)));
            if(availableProject is null)
                return Unauthorized("You havent access to this action");

            var canbanBoard = await _context.CanbanBoards.FindAsync(newCanbanBoard.Id);
            if (canbanBoard is null)
                return NotFound($"Not found canban board with {newCanbanBoard.Id} id");
            if (canbanBoard.BaseBoardId != newCanbanBoard.BaseBoardId)
                return UnprocessableEntity($"CanbanBoard doesnt have BaseBoard with {newCanbanBoard.BaseBoardId} id");
            canbanBoard.TaskLimit = newCanbanBoard.TaskLimit;

            var baseBoard = await _context.BaseBoards.FindAsync(newCanbanBoard.BaseBoardId);
            if (baseBoard is null)
                return NotFound($"Not found base board with {newCanbanBoard.BaseBoardId} id");
            baseBoard.Name = newCanbanBoard.BaseBoard.Name;
            baseBoard.Description = newCanbanBoard.BaseBoard.Description;

            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpPut("updateScrumBoard")]
        public async Task<IActionResult> UpdateScrumBoard(ScrumBoardDTO newScrumBoard)
        {
            var availableProject = await _context.Projects
                .Include(p => p.Participants)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == newScrumBoard.BaseBoard.ProjectId &&
                    p.Participants.Any(p => p.UserId == _userId && new[] { 1, 2 }.Contains(p.RoleId)));
            if (availableProject is null)
                return Unauthorized("You havent access to this action");

            var scrumBoard = await _context.ScrumBoards.FindAsync(newScrumBoard.Id);
            if (scrumBoard is null)
                return NotFound($"Not found scrum board with {newScrumBoard.Id} id");
            if (scrumBoard.BaseBoardId != newScrumBoard.BaseBoardId)
                return UnprocessableEntity($"CanbanBoard doesnt have BaseBoard with {newScrumBoard.BaseBoardId} id");
            scrumBoard.TimeLimit = newScrumBoard.TimeLimit;
            
            var baseBoard = await _context.BaseBoards.FindAsync(newScrumBoard.BaseBoardId);
            if (baseBoard is null)
                return NotFound($"Not found base board with {newScrumBoard.BaseBoard.Id} id");
            baseBoard.Name = newScrumBoard.BaseBoard.Name;
            baseBoard.Description = newScrumBoard.BaseBoard.Description;

            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int baseBoardId)
        {
            var availableBoard = await _context.BaseBoards
                .Include(p => p.Project)
                .ThenInclude(p => p.Participants)
                .AsNoTracking()
                .FirstOrDefaultAsync(bb => bb.Id == baseBoardId && 
                    bb.Project.Participants.Any(p => p.UserId == _userId && p.RoleId == 1));
            if (availableBoard is null)
                return Unauthorized("You havent access to this action");

            var baseBoard = await _context.BaseBoards.FindAsync(baseBoardId);
            if (baseBoard is null)
                return NotFound("Not found base board");
            
            var scrumBoard = await _context.ScrumBoards.FirstOrDefaultAsync(sb => sb.BaseBoardId == baseBoardId);
            var canbanBoard = await _context.CanbanBoards.FirstOrDefaultAsync(cb => cb.BaseBoardId == baseBoardId);

            if (scrumBoard is null && canbanBoard is null)
                return NotFound("Not found scrum and canban boards");

            _context.BaseBoards.Remove(baseBoard);
            if (scrumBoard is null)
                _context.CanbanBoards.Remove(canbanBoard);
            _context.ScrumBoards.Remove(scrumBoard);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
