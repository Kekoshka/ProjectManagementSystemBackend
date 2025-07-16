using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Interfaces;
using ProjectManagementSystemBackend.Models.DTO;
using System.Net.WebSockets;
using System.Security.Claims;
using IAuthorizationService = ProjectManagementSystemBackend.Interfaces.IAuthorizationService;

namespace ProjectManagementSystemBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TasksController : ControllerBase
    {
        ITaskService _taskService;
        IAuthorizationService _authorizationService;
        ITaskHistoryService _taskHistoryService;
        int? userId;
        int[] _userRoles = [1, 2, 3];
        int[] _adminRoles = [1, 2];
        int[] _ownerRoles = [1];
        int _userId => userId ??= Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)); 
        public TasksController(ITaskService taskService, ITaskHistoryService taskHistoryService, IAuthorizationService authorizationService) 
        {
            _taskService = taskService;
            _taskHistoryService = taskHistoryService;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync(int boardStatusId,CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByBoardStatusIdAsync(boardStatusId, _userId, _userRoles, cancellationToken);
            if(!isAuthorized)    
                return Unauthorized("You havent access to this action");

            var tasks = await _taskService.GetAsync(boardStatusId, cancellationToken);
            return tasks is null ? NotFound() : Ok(tasks);
        }
        [HttpPost]
        public async Task<IActionResult> PostAsync(TaskDTO task,CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByBoardStatusIdAsync(task.BoardStatusId, _userId, _adminRoles, cancellationToken);
            if(!isAuthorized)
                return Unauthorized("You havent access to this action");

            try
            {
                var newTask = await _taskService.PostAsync(task, _userId, cancellationToken);
                return Ok(newTask);
            }
            catch (InvalidDataException ex) { return BadRequest(ex.Message); }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
            catch (Exception) { return StatusCode(500, "Internal server error"); }
        }
        [HttpPut]
        public async Task<IActionResult> UpdateAsync(TaskDTO updatedTask,CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByTaskIdAsync(updatedTask.Id, _userId, _adminRoles, cancellationToken);
            if(!isAuthorized)
                return Unauthorized("You havent access to this action");

            try
            {
                await _taskService.UpdateAsync(updatedTask, _userId, cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
            catch (InvalidDataException ex) { return BadRequest(ex.Message); }
            catch (Exception) { return StatusCode(500, "Internal server error"); }
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(int taskId,CancellationToken cancellationToken)
        {
            bool isAuthorize = await _authorizationService.AccessByTaskIdAsync(taskId, _userId, _adminRoles, cancellationToken);
            if(!isAuthorize)
                return Unauthorized("You havent access to this action");

            try
            {
                await _taskService.DeleteAsync(taskId,cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException) { return NotFound(); }
            catch (Exception) { return StatusCode(500, "Internal server error"); }
        }

    }
}
