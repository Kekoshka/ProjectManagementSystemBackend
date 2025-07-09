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

        int? userId;
        int _userId => userId ??= Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        int[] _userRoles = [1, 2, 3];
        int[] _adminRoles = [1, 2];
        int[] _ownerRoles = [1];

        public ParticipantsController(ApplicationContext context, IAuthorizationService authorizationService) 
        {
            _context = context;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync(int projectId, CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByProjectIdAsync(projectId, _userId, _userRoles, cancellationToken);
            if (!isAuthorized)
                return Unauthorized("You havent access to this action");

            var participants = await _context.Participants
                .Include(p => p.User)
                .Where(p => p.ProjectId == projectId)
                .ProjectToType<ParticipantDTO>()
                .ToListAsync(cancellationToken);

            return participants is null ? NotFound() : Ok(participants);
        }
        [HttpPost]
        public async Task<IActionResult> PostAsync(ParticipantDTO participant, CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByProjectIdAsync(participant.ProjectId, _userId, _ownerRoles, cancellationToken);
            if (!isAuthorized)
                return Unauthorized("You havent access to this action");

            if (!_userRoles.Any(r => r == participant.RoleId))
                return BadRequest("Invalid role id");
            var existUser = await _context.Users.FindAsync(participant.UserId);
            if (existUser is null)
                return BadRequest("Invalid user id");

            var existParticipant = await _context.Participants
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProjectId == participant.ProjectId && 
                    p.UserId == participant.UserId);
            if (existParticipant is not null)
                return Conflict("User is already a participant of the project");

            Participant newParticipant = participant.Adapt<Participant>();
            await _context.Participants.AddAsync(newParticipant);
            await _context.SaveChangesAsync();


            return Ok(newParticipant.Id);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateAsync(ParticipantDTO newParticipant,CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByParticipantIdAsync(newParticipant.Id, _userId, _ownerRoles, cancellationToken);
            if (!isAuthorized)
                return Unauthorized("You havent access to this action");

            if (!_userRoles.Any(r => r == newParticipant.RoleId))
                return BadRequest("Invalid role id");

            var participant = await _context.Participants.FindAsync(newParticipant.Id);
            if (participant is null)
                return NotFound($"Participant with id {newParticipant.Id} id not found");

            participant.RoleId = newParticipant.RoleId;
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(int participantId, CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByParticipantIdAsync(participantId, _userId, _ownerRoles, cancellationToken);
            if (!isAuthorized)
                return Unauthorized("You havent access to this action");

            var participant = await _context.Participants
                .FirstOrDefaultAsync(p => p.Id == participantId);
            if (participant is null)
                return NotFound();
            _context.Participants.Remove(participant);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
