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
    public class ParticipantsController : ControllerBase
    {
        ApplicationContext _context;
        int? userId;
        int _userId => userId ??= Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)); public ParticipantsController(ApplicationContext context) 
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int projectId)
        {
            var availableProject = await _context.Projects
                .Include(p => p.Participants)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == projectId && 
                    (p.Security == false || p.Participants.Any(p => p.UserId == _userId)));
            if (availableProject is null)
                return Unauthorized("You havent access to this action");

            var participants = await _context.Participants
                .Include(p => p.User)
                .Where(p => p.ProjectId == projectId)
                .Select(p => new ParticipantDTO()
                {
                    Id = p.Id,
                    ProjectId = p.ProjectId,
                    RoleId = p.RoleId,
                    UserId = _userId,
                    User = new UserDTO()
                    {
                        Id = p.User.Id,
                        Name = p.User.Name
                    }
                })
                .AsNoTracking()
                .ToListAsync();
            return Ok(participants);
        }
        [HttpPost]
        public async Task<IActionResult> Post(ParticipantDTO participant)
        {
            var availableProject = await _context.Projects
                .Include(p => p.Participants)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == participant.ProjectId &&
                    p.Participants.Any(p => p.UserId == _userId && p.RoleId == 1));
            if (availableProject is null)
                return Unauthorized("You havent access to this action");
            if (!new int[] { 1, 2, 3 }.Any(r => r == participant.RoleId))
                return BadRequest("Invalid role id");
            var existUser = await _context.Users.FindAsync(participant.UserId);
            if (existUser is null)
                return BadRequest("Invalid user id");
            var existProject = await _context.Projects.FindAsync(participant.ProjectId);
            if (existProject is null)
                return BadRequest("invalid project id");
            var existParticipant = await _context.Participants
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProjectId == participant.ProjectId && 
                    p.UserId == participant.UserId);
            if (existParticipant is not null)
                return Conflict("User is already a participant of the project");

            Participant newParticipant = new()
            {
                UserId = participant.UserId,
                ProjectId = participant.ProjectId,
                RoleId = participant.RoleId
            };
            await _context.Participants.AddAsync(newParticipant);
            await _context.SaveChangesAsync();

            return Ok(newParticipant.Id);
        }
        [HttpPut]
        public async Task<IActionResult> Update(ParticipantDTO newParticipant)
        {
            var availableProject = await _context.Projects
                .Include(p => p.Participants)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == newParticipant.ProjectId &&
                    p.Participants.Any(p => p.UserId == _userId && p.RoleId == 1));
            if (availableProject is null)
                return Unauthorized("You havent access to this action");
            if (!new int[] {1,2,3}.Any(r => r == newParticipant.RoleId))
                return BadRequest("Invalid role id");

            var participant = await _context.Participants.FindAsync(newParticipant.Id);
            if (participant is null)
                return NotFound($"Participant with id {newParticipant.Id} id not found");

            participant.RoleId = newParticipant.RoleId;
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int participantId)
        {
            var availableProject = await _context.Projects
                .Include(p => p.Participants)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Participants.Any(p => p.Id == participantId)
                    && p.Participants.Any(p => p.UserId == _userId && p.RoleId == 1));
            if (availableProject is null)
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
