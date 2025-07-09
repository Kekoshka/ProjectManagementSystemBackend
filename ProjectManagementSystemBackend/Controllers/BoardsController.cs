using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Interfaces;
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
        Interfaces.IAuthorizationService _authorizationService;

        int? userId;
        int _userId => userId ??= Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        int[] _userRoles = [1, 2, 3];
        int[] _adminRoles = [1, 2];
        int[] _ownerRoles = [1];

        public BoardsController(ApplicationContext context, Interfaces.IAuthorizationService authorizationService)
        {
            _context = context;
            _authorizationService = authorizationService;
        }
        [HttpGet("getBaseBoardsByProjectId")]
        public async Task<IActionResult> GetBoardsByProjectIdAsync(int projectId, CancellationToken cancellationToken)
        {
            var IsAuthorize = await _authorizationService.AccessByProjectIdAsync(projectId, _userId, _userRoles, cancellationToken);
            if (!IsAuthorize)
                return Unauthorized("You havent access to this action");

            var boards = await _context.BaseBoards
                .Where(b => b.ProjectId == projectId)
                .ProjectToType<BaseBoardDTO>()
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return boards is null ? NotFound() : Ok(boards);
        }
        [HttpGet("getBoardByBaseBoardId")]
        public async Task<IActionResult> GetByBaseBoardIdAsync(int baseBoardId, CancellationToken cancellationToken)
        {
            bool IsAuthorize = await _authorizationService.AccessByBoardIdAsync(baseBoardId, _userId, _userRoles, cancellationToken);
            if (!IsAuthorize)
                return Unauthorized("You havent access to this action");

            var scrumBoard = await _context.ScrumBoards
                .AsNoTracking()
                .ProjectToType<ScrumBoardDTO>()
                .FirstOrDefaultAsync(sb => sb.BaseBoardId == baseBoardId);
            if (scrumBoard is not null)
                return Ok(scrumBoard);

            var canbanBoard = await _context.CanbanBoards
                .AsNoTracking()
                .ProjectToType<CanbanBoardDTO>()
                .FirstOrDefaultAsync(cb => cb.BaseBoardId == baseBoardId);
            return canbanBoard is null ? NotFound() : Ok(canbanBoard);
        }
        [HttpPost("postCanbanBoard")]
        public async Task<IActionResult> PostCanbanAsync(CanbanBoardDTO canbanBoard, CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByBoardIdAsync(canbanBoard.BaseBoard.Id, _userId, _adminRoles, cancellationToken);
            if (!isAuthorized)
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
        public async Task<IActionResult> PostScrumAsync(ScrumBoardDTO scrumBoard, CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByProjectIdAsync(scrumBoard.BaseBoard.ProjectId, _userId, _adminRoles, cancellationToken);
            if (!isAuthorized)
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
        public async Task<IActionResult> UpdateCanbanBoardAsync(CanbanBoardDTO newCanbanBoard, CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByProjectIdAsync(newCanbanBoard.BaseBoard.ProjectId, _userId, _adminRoles, cancellationToken);
            if (!isAuthorized)
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
        public async Task<IActionResult> UpdateScrumBoardAsync(ScrumBoardDTO newScrumBoard, CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByProjectIdAsync(newScrumBoard.BaseBoard.ProjectId, _userId, _adminRoles, cancellationToken);
            if (!isAuthorized)
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
        public async Task<IActionResult> DeleteAsync(int baseBoardId, CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByBoardIdAsync(baseBoardId, _userId, _ownerRoles, cancellationToken);
            if (!isAuthorized)
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
