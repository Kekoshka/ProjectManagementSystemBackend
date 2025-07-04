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
    public class ParticipantsController : ControllerBase
    {
        ApplicationContext _context;
        int _userId;
        public ParticipantsController(ApplicationContext context) 
        {
            _context = context;
            _userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        [HttpGet("getByProjectId")]
        public async Task<IActionResult> GetByProjectId(int projectId)
        {
            var availableProject = _context.Projects
                .Include(p => p.Participants)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == projectId && 
                    (p.Security == false || p.Participants.Any(p => p.UserId == _userId)));
            if (availableProject is null)
                return Unauthorized("You havent access to this action");

            var participants = _context.Participants
                .Include(p => p.User)
                .Where(p => p.ProjectId == projectId)
                .Select(p => new ParticipantDTO()
                {
                    Id = p.Id,
                    ProjectId = p.Id,
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
            var availableProject = _context.Projects
                .Include(p => p.Participants)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == participant.ProjectId &&
                    p.Participants.Any(p => p.UserId == _userId && p.RoleId == 1));
            if (availableProject is null)
                return Unauthorized("You havent access to this action");
            
            Participant newParticipant = new()
            {
                UserId = participant.UserId,
                ProjectId = participant.ProjectId,
                RoleId = participant.RoleId
            };
            _context.Participants.Add(newParticipant);
            await _context.SaveChangesAsync();

            return Ok(newParticipant);
        }
        [HttpPut]
        public async Task<IActionResult> Update(ParticipantDTO newParticipant)
        {
            var availableProject = _context.Projects
                .Include(p => p.Participants)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == newParticipant.ProjectId &&
                    p.Participants.Any(p => p.UserId == _userId && p.RoleId == 1));
            if (availableProject is null)
                return Unauthorized("You havent access to this action");

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
