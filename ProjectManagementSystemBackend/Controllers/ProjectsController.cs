using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Identity.Client;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Interfaces;
using ProjectManagementSystemBackend.Models;
using ProjectManagementSystemBackend.Models.DTO;
using System.Security.Claims;
using IAuthorizationService = ProjectManagementSystemBackend.Interfaces.IAuthorizationService;

namespace ProjectManagementSystemBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        ApplicationContext _context;
        IAuthorizationService _authorizationService;

        int? userId;
        int _userId => userId ??= Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        int[] _userRoles = [1, 2, 3];
        int[] _adminRoles = [1, 2];
        int[] _ownerRoles = [1];
        public ProjectsController(ApplicationContext context, IAuthorizationService authorizationService) 
        {
            _context = context;
            _authorizationService = authorizationService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var projects = await _context.Projects
                .Include(p => p.Participants)
                .Where(p => p.Security == false || p.Participants.Any(p => p.UserId == _userId))
                .AsNoTracking()
                .ToListAsync();

            List<ProjectDTO> projectsDTO = projects.Select(p => new ProjectDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Security = p.Security
            }).ToList();

            return projects is null ? NotFound() : Ok(projectsDTO);
        }
        [HttpPost]
        public async Task<IActionResult> CreateAsync(ProjectDTO project)
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

            return Ok(newProject.Id);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync(ProjectDTO newProject, CancellationToken cancellationToken)
        {
            bool isAuthorize = await _authorizationService.AccessByProjectIdAsync(newProject.Id, _userId, _ownerRoles, cancellationToken);
            if(!isAuthorize)
                return Unauthorized("You havent access to this action");

            var updatingProject = await _context.Projects.FirstOrDefaultAsync(p => p.Id == newProject.Id);
            if (updatingProject is null)
                return NotFound();

            updatingProject.Name = newProject.Name;
            updatingProject.Description = newProject.Description;
            updatingProject.Security = newProject.Security;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(int projectId, CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByProjectIdAsync(projectId, _userId, _ownerRoles, cancellationToken);
            if(!isAuthorized)
                return Unauthorized("You havent access to this action");

            var beingDeletedProject = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
            if (beingDeletedProject is null)
                return NotFound();

            _context.Remove(beingDeletedProject);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
