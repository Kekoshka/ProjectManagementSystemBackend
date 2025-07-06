using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Models;
using ProjectManagementSystemBackend.Models.DTO;
using System.ComponentModel;
using System.Security.Claims;

namespace ProjectManagementSystemBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StatusesController : ControllerBase
    {
        ApplicationContext _context;
        int? userId;
        int _userId => userId ??= Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)); public StatusesController(ApplicationContext context) 
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int baseBoardId)
        {
            var availableBoard = await _context.BaseBoards
                .Include(bb => bb.Project)
                .ThenInclude(bb => bb.Participants)
                .FirstOrDefaultAsync(bb => bb.Id == baseBoardId && 
                (bb.Project.Security == false || bb.Project.Participants.Any(p => p.UserId == _userId)));
            if (availableBoard is null)
                return Unauthorized("You havent access to this action");

            var statuses = await _context.BoardStatuses
                .Include(bs => bs.Status)
                .Where(bs => bs.BaseBoardId == baseBoardId)
                .Select(bs => new BoardStatus
                {
                    Id = bs.Id,
                    BaseBoardId = bs.BaseBoardId,
                    StatusId = bs.StatusId,
                    Status = new Status
                    {
                        Id = bs.StatusId,
                        Name = bs.Status.Name,
                    }
                }).ToListAsync();
            return statuses is null ? NotFound() : Ok(statuses);
        }
        [HttpPost]
        public async Task<IActionResult> Post(StatusDTO status)
        {
            var availableBaseBoard = await _context.BaseBoards
                .Include(bb => bb.Project)
                .ThenInclude(bb => bb.Participants)
                .AsNoTracking()
                .FirstOrDefaultAsync(bb => bb.Id == status.BaseBoardId &&
                bb.Project.Participants.Any(p => p.UserId == _userId && new int[] { 1, 2 }.Any(r => r == p.RoleId)));
            if (availableBaseBoard is null)
                return Unauthorized("You havent access to this action");

            var existsStatus = await _context.Statuses.FirstOrDefaultAsync(s => s.Name == status.Name);
            if(existsStatus is null)
            {
                Status newStatus = new()
                {
                    Name = status.Name
                };
                _context.Statuses.Add(newStatus);
                _context.SaveChanges();
                existsStatus = newStatus;
            }
            BoardStatus newBoardStatus = new()
            {
                BaseBoardId = status.BaseBoardId,
                StatusId = existsStatus.Id,
            };
            await _context.BoardStatuses.AddAsync(newBoardStatus);
            await _context.SaveChangesAsync();

            StatusDTO newBoardStatusDTO = new() { 
                Id = newBoardStatus.Id,
                Name = newBoardStatus.Status.Name,
                BaseBoardId = newBoardStatus.BaseBoardId, 
                StatusId = newBoardStatus.StatusId };
            return Ok(newBoardStatusDTO);
        }
        [HttpPut]
        public async Task<IActionResult> Update(StatusDTO updatedStatus)
        {
            var availableBoardStatus = await _context.BoardStatuses
                .Include(bs => bs.BaseBoard)
                .ThenInclude(bs => bs.Project)
                .ThenInclude(p => p.Participants)
                .FirstOrDefaultAsync(s => s.Id == updatedStatus.Id && 
                    s.BaseBoard.Project.Participants.Any(p => p.UserId == _userId && new int[] { 1, 2 }.Any(r => r == p.RoleId)));
            if (availableBoardStatus is null)
                return Unauthorized("You havent access to this action");

            var existedStatus = await _context.Statuses.FirstOrDefaultAsync(s => s.Name == updatedStatus.Name);
            if(existedStatus is null)
            {
                Status newStatus = new()
                {
                    Name = updatedStatus.Name
                };
                await _context.Statuses.AddAsync(newStatus);
                await _context.SaveChangesAsync();
                existedStatus = newStatus;
            }
            availableBoardStatus.StatusId = existedStatus.Id;
            await _context.SaveChangesAsync();

            StatusDTO statusDTO = new()
            {
                Name = existedStatus.Name,
                StatusId = availableBoardStatus.StatusId,
                BaseBoardId = availableBoardStatus.BaseBoardId,
                Id = availableBoardStatus.Id
            };

            return NoContent();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int boardStatusId)
        {
            var availableBoardStatus = await _context.BoardStatuses
                .Include(bs => bs.BaseBoard)
                .ThenInclude(bb => bb.Project)
                .ThenInclude(p => p.Participants)
                .AsNoTracking()
                .FirstOrDefaultAsync(bs => bs.Id == boardStatusId &&
                    bs.BaseBoard.Project.Participants.Any(p => p.UserId == _userId && new int[] { 1, 2 }.Any(r => r == p.RoleId)));
            if (availableBoardStatus is null)
                return Unauthorized("You havent access to this action");

            var status = await _context.BoardStatuses.FindAsync(boardStatusId);
            if(status is null)
                return NotFound();
            _context.BoardStatuses.Remove(status);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
