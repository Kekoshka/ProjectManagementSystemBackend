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
        IAuthorizationService _authorizationService;
        IProjectService _projectService;

        int? userId;
        int _userId => userId ??= Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        int[] _userRoles = [1, 2, 3];
        int[] _adminRoles = [1, 2];
        int[] _ownerRoles = [1];
        public ProjectsController(IProjectService projectService, IAuthorizationService authorizationService) 
        {
            _projectService = projectService;
            _authorizationService = authorizationService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
        {
            var projects = await _projectService.GetAsync(_userId, cancellationToken);
            return projects is null ? NotFound() : Ok(projects);
        }
        [HttpPost]
        public async Task<IActionResult> CreateAsync(ProjectDTO project, CancellationToken cancellationToken)
        {
            var newProject = await _projectService.CreateAsync(project, _userId, cancellationToken);

            return Ok(newProject.Id);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync(ProjectDTO newProject, CancellationToken cancellationToken)
        {
            bool isAuthorize = await _authorizationService.AccessByProjectIdAsync(newProject.Id, _userId, _ownerRoles, cancellationToken);
            if(!isAuthorize)
                return Unauthorized("You havent access to this action");

            try
            {
                await _projectService.UpdateAsync(newProject, cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException) { return NotFound(); }
            catch (Exception) { return StatusCode(500, "Internal server error"); }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(int projectId, CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByProjectIdAsync(projectId, _userId, _ownerRoles, cancellationToken);
            if(!isAuthorized)
                return Unauthorized("You havent access to this action");

            try
            {
                await _projectService.DeleteAsync(projectId, cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException) { return NotFound(); }
            catch (Exception) { return StatusCode(500, "Internal server error"); }
        }
    }
}
