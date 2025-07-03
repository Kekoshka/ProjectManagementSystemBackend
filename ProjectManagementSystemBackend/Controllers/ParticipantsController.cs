using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystemBackend.Context;
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

    }
}
