using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Interfaces;
using ProjectManagementSystemBackend.Models;
using ProjectManagementSystemBackend.Models.DTO;
using System.ComponentModel;
using System.Security.Claims;
using IAuthorizationService = ProjectManagementSystemBackend.Interfaces.IAuthorizationService;

namespace ProjectManagementSystemBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StatusesController : ControllerBase
    {
        ApplicationContext _context;
        IAuthorizationService _authorizationService;

        int? userId;
        int _userId => userId ??= Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        int[] _userRoles = [1, 2, 3];
        int[] _adminRoles = [1, 2];
        int[] _ownerRoles = [1];
        public StatusesController(ApplicationContext context, IAuthorizationService authorizationService) 
        {
            _context = context;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync(int baseBoardId, CancellationToken cancellationToken)
        {
            bool isAuthorize = await _authorizationService.AccessByBoardIdAsync(baseBoardId, _userId, _userRoles, cancellationToken);
            if (!isAuthorize)
                return Unauthorized("You havent access to this action");

            var statuses = await _context.BoardStatuses
                .Include(bs => bs.Status)
                .Where(bs => bs.BaseBoardId == baseBoardId)
                .ProjectToType<StatusDTO>()
                .ToListAsync(cancellationToken);

            return statuses is null ? NotFound() : Ok(statuses);
        }
        [HttpPost]
        public async Task<IActionResult> PostAsync(StatusDTO status, CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByBoardIdAsync(status.BaseBoardId, _userId, _adminRoles, cancellationToken);
            if (!isAuthorized)
                return Unauthorized("You havent access to this action");

            var existsStatus = await _context.Statuses.FirstOrDefaultAsync(s => s.Name == status.Name);
            if(existsStatus is null)
            {
                Status newStatus = new() { Name = status.Name };
                await _context.Statuses.AddAsync(newStatus,cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                existsStatus = newStatus;
            }
            BoardStatus newBoardStatus = new()
            {
                BaseBoardId = status.BaseBoardId,
                StatusId = existsStatus.Id,
            };
            await _context.BoardStatuses.AddAsync(newBoardStatus,cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            StatusDTO newBoardStatusDTO = newBoardStatus.Adapt<StatusDTO>();
            return Ok(newBoardStatusDTO);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateAsync(StatusDTO status, CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByBoardStatusIdAsync(status.Id, _userId, _adminRoles, cancellationToken);
            if(!isAuthorized)
                return Unauthorized("You havent access to this action");

            var updatedStatus = await _context.BoardStatuses.FindAsync(status.Id);
            if (updatedStatus is null)
                return NotFound();

                var existedStatus = await _context.Statuses.FirstOrDefaultAsync(s => s.Name == status.Name);
            if(existedStatus is null)
            {
                Status newStatus = new()
                {
                    Name = status.Name
                };
                await _context.Statuses.AddAsync(newStatus);
                await _context.SaveChangesAsync();
                existedStatus = newStatus;
            }

            updatedStatus.StatusId = existedStatus.Id;
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(int boardStatusId, CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByBoardStatusIdAsync(boardStatusId, _userId, _adminRoles, cancellationToken);
            if(!isAuthorized)    
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
