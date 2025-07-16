using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Interfaces;
using ProjectManagementSystemBackend.Models;
using ProjectManagementSystemBackend.Models.DTO;
using System;
using System.Security.Claims;
using IAuthorizationService = ProjectManagementSystemBackend.Interfaces.IAuthorizationService;

namespace ProjectManagementSystemBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ParticipantsController : ControllerBase
    {
        ApplicationContext _context;
        IAuthorizationService _authorizationService;
        IParticipantService _participantService;

        int? userId;
        int _userId => userId ??= Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        int[] _userRoles = [1, 2, 3];
        int[] _adminRoles = [1, 2];
        int[] _ownerRoles = [1];

        public ParticipantsController(IParticipantService participantService, IAuthorizationService authorizationService) 
        {
            _participantService = participantService;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync(int projectId, CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByProjectIdAsync(projectId, _userId, _userRoles, cancellationToken);
            if (!isAuthorized)
                return Unauthorized("You havent access to this action");

            var participants = await _participantService.GetAsync(projectId,cancellationToken);
            return participants is null ? NotFound() : Ok(participants);
        }
        [HttpPost]
        public async Task<IActionResult> PostAsync(ParticipantDTO participant, CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByProjectIdAsync(participant.ProjectId, _userId, _ownerRoles, cancellationToken);
            if (!isAuthorized)
                return Unauthorized("You havent access to this action");
            try
            {
                var newParticipant = await _participantService.PostAsync(participant, cancellationToken);
                return Ok(newParticipant.Id);
            }
            catch (InvalidDataException ex) { return BadRequest(ex.Message); }
            catch (InvalidOperationException ex) { return Conflict(ex.Message); }
            catch (Exception) { return StatusCode(500, "Internal server error"); }

        }
        [HttpPut]
        public async Task<IActionResult> UpdateAsync(ParticipantDTO newParticipant,CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByParticipantIdAsync(newParticipant.Id, _userId, _ownerRoles, cancellationToken);
            if (!isAuthorized)
                return Unauthorized("You havent access to this action");

            try
            {
                await _participantService.UpdateAsync(newParticipant, cancellationToken);
                return NoContent();
            }
            catch (InvalidDataException ex) { return BadRequest(ex.Message); }
            catch (Exception) { return StatusCode(500, "Internal server error"); }
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(int participantId, CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByParticipantIdAsync(participantId, _userId, _ownerRoles, cancellationToken);
            if (!isAuthorized)
                return Unauthorized("You havent access to this action");

            try
            {
                await _participantService.DeleteAsync(participantId, cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException) { return NotFound(); }
            catch (Exception) { return StatusCode(500, "Internal server error"); }

        }
    }
}
