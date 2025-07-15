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
        IStatusService _statusService;
        IAuthorizationService _authorizationService;

        int? userId;
        int _userId => userId ??= Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        int[] _userRoles = [1, 2, 3];
        int[] _adminRoles = [1, 2];
        int[] _ownerRoles = [1];
        public StatusesController(IStatusService statusService, IAuthorizationService authorizationService) 
        {
            _statusService = statusService;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync(int baseBoardId, CancellationToken cancellationToken)
        {
            bool isAuthorize = await _authorizationService.AccessByBoardIdAsync(baseBoardId, _userId, _userRoles, cancellationToken);
            if (!isAuthorize)
                return Unauthorized("You havent access to this action");

            var statuses = _statusService.GetAsync(baseBoardId, cancellationToken);
            return statuses is null ? NotFound() : Ok(statuses);
        }
        [HttpPost]
        public async Task<IActionResult> PostAsync(StatusDTO status, CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByBoardIdAsync(status.BaseBoardId, _userId, _adminRoles, cancellationToken);
            if (!isAuthorized)
                return Unauthorized("You havent access to this action");

            var newStatus = await _statusService.PostAsync(status, cancellationToken);
            return Ok(newStatus);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateAsync(StatusDTO status, CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByBoardStatusIdAsync(status.Id, _userId, _adminRoles, cancellationToken);
            if(!isAuthorized)
                return Unauthorized("You havent access to this action");

            try
            {
                await _statusService.UpdateAsync(status, cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException) { return NotFound(); }
            catch (Exception) { return StatusCode(500, "Internal server error"); }
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(int boardStatusId, CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByBoardStatusIdAsync(boardStatusId, _userId, _adminRoles, cancellationToken);
            if(!isAuthorized)    
                return Unauthorized("You havent access to this action");

            try
            {
                await _statusService.DeleteAsync(boardStatusId, cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException) { return NotFound(); }
            catch (Exception) { return StatusCode(500, "Internal server error"); }
        }
    }
}
