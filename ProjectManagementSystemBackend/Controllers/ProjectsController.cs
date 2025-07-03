using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Identity.Client;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Models;
using ProjectManagementSystemBackend.Models.DTO;
using System.Security.Claims;

namespace ProjectManagementSystemBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        ApplicationContext _context;
        int _userId;
        public ProjectsController(ApplicationContext context) 
        {
            _context = context;
            _userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }
        [HttpGet("getAll")]
        public async Task<IActionResult> Get()
        {
            var projects = _context.Projects
                .Include(p => p.Participants)
                .Where(p => p.Security == false || p.Participants.Any(p => p.UserId == _userId))
                .AsNoTracking()
                .ToListAsync();
            
            return projects is null ? NotFound() : Ok(projects);
        }
        [HttpGet("getById")]
        public async Task<IActionResult> GetById(int projectId)
        {
            var project = await _context.Projects
                .Include(p => p.Participants)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == projectId
                && (p.Security == false
                || p.Participants.Any(p => p.UserId == _userId)));

            return project is null ? NotFound() : Ok(project); 
        }
        [HttpPost]
        public async Task<IActionResult> Create(ProjectDTO project)
        {
            Project newProject = new()
            {
                Name = project.Name,
                Description = project.Description,
                Security = project.Security
            };

            _context.Projects.Add(newProject);
            await _context.SaveChangesAsync();

            Participant newParticipant = new()
            {
                ProjectId = newProject.Id,
                UserId = _userId,
                RoleId = 1
            };
            newProject.Participants = [newParticipant];
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut]
        public async Task<IActionResult> Update(ProjectDTO newProject)
        {
            var existingUser = _context.Participants
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.UserId == _userId 
                    && p.ProjectId == newProject.Id 
                    && new int[] { 1, 2 }.Contains(p.UserId));
            if (existingUser is null)
                return Unauthorized("you havent access to this action");

            var updatingProject = await _context.Projects.FirstOrDefaultAsync(p => p.Id == newProject.Id);
            if (updatingProject is null)
                return NotFound();

            updatingProject.Name = newProject.Name;
            updatingProject.Description = newProject.Description;
            updatingProject.Security = newProject.Security;

            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int projectId)
        {
            var existsParticipant = await _context.Participants
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProjectId == projectId
                    && p.RoleId == 1
                    && p.UserId == _userId);
            if (existsParticipant is null)
                return Unauthorized("you havent access to this action");

            var beingDeletedProject = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
            if (beingDeletedProject is null)
                return NotFound();

            _context.Remove(beingDeletedProject);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
