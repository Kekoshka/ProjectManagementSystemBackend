using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Interfaces;
using ProjectManagementSystemBackend.Models.DTO;
using System.Security.Claims;
using IAuthorizationService = ProjectManagementSystemBackend.Interfaces.IAuthorizationService;

namespace ProjectManagementSystemBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TasksController : ControllerBase
    {
        ApplicationContext _context;
        IAuthorizationService _authorizationService;
        ITaskHistoryService _taskHistoryService;
        int? userId;
        int[] _userRoles = [1, 2, 3];
        int[] _adminRoles = [1, 2];
        int[] _ownerRoles = [1];
        int _userId => userId ??= Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)); 
        public TasksController(ApplicationContext context, ITaskHistoryService taskHistoryService, IAuthorizationService authorizationService) 
        {
            _context = context;
            _taskHistoryService = taskHistoryService;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync(int baseBoadrId,CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByBoardIdAsync(baseBoadrId, _userId, _userRoles, cancellationToken);
            if(!isAuthorized)    
                return Unauthorized("You havent access to this action");

            var baseBoard = await _context.BaseBoards.Include(bb => bb.BoardStatuses).ThenInclude(bs => bs.Tasks).FirstOrDefaultAsync(bb => bb.Id == baseBoadrId);
            if(baseBoard is null) 
                return NotFound();
            var tasks = baseBoard.BoardStatuses.Select(bs => bs.Tasks);

            return tasks is null ? NotFound() : Ok(tasks);
        }
        [HttpPost]
        public async Task<IActionResult> PostAsync(TaskDTO task,CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByBoardStatusIdAsync(task.BoardStatusId, _userId, _adminRoles, cancellationToken);
            if(!isAuthorized)
                return Unauthorized("You havent access to this action");

            var existParticipant = _context.BoardStatuses
                .Where(bs => bs.Id == task.BoardStatusId && 
                bs.BaseBoard.Project.Participants.Any(p => p.UserId == ))
            if (currentParticipantId is 0)
                return NotFound("Participant not found");
            var existResponsiblePerson = await _context.Participants.FindAsync(task.ResponsiblePersonId);
            if (existResponsiblePerson is null)
                return BadRequest("Invalid responsible person id");

            Models.Task newTask = new(task.Name,
                task.Description,
                task.Priority,
                DateTime.UtcNow,
                task.TimeLimit, 
                currentParticipantId, 
                task.ResponsiblePersonId,
                task.BoardStatusId);

            await _context.Tasks.AddAsync(newTask);
            await _context.SaveChangesAsync();

            await _taskHistoryService.CreateAsync(newTask, _userId);

            TaskDTO taskDTO = new()
            {
                BoardStatusId = newTask.BoardStatusId,
                CreatorId = newTask.CreatorId,
                Description = newTask.Description,
                Id = newTask.Id,
                LastUpdate = newTask.LastUpdate,
                Name = newTask.Name,
                Priority = newTask.Priority,
                ResponsiblePersonId = newTask.ResponsiblePersonId,
                TimeLimit = newTask.TimeLimit
            };
            return Ok(taskDTO);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateAsync(TaskDTO updatedTask,CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByTaskIdAsync(updatedTask.Id, _userId, _adminRoles, cancellationToken);
            if(!isAuthorized)
                return Unauthorized("You havent access to this action");
             
            var existBoardStauts = await _context.BoardStatuses.FindAsync(updatedTask.BoardStatusId);
            if (existBoardStauts is null)
                return BadRequest("Invalid board status id");

            var existResponsiblePerson = await _context.Participants.FindAsync(updatedTask.ResponsiblePersonId);
            if (existResponsiblePerson is null)
                return BadRequest("Invalid responsible person id");
                
            var oldTask = await _context.Tasks.FindAsync(updatedTask.Id);
            var task = await _context.Tasks.FindAsync(updatedTask.Id);
            if (task is null)
                return NotFound("Task not found");
            task.Name = updatedTask.Name;
            task.Description = updatedTask.Description;
            task.Priority = updatedTask.Priority;
            task.LastUpdate = DateTime.UtcNow;
            task.TimeLimit = updatedTask.TimeLimit;
            task.ResponsiblePersonId = updatedTask.ResponsiblePersonId;
            task.BoardStatusId = updatedTask.BoardStatusId;

            await _context.SaveChangesAsync();

            await _taskHistoryService.UpdateAsync(oldTask, task, _userId);

            return NoContent();
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(int taskId,CancellationToken cancellationToken)
        {
            bool isAuthorize = await _authorizationService.AccessByTaskIdAsync(taskId, _userId, _adminRoles, cancellationToken);
            if(!isAuthorize)
                return Unauthorized("You havent access to this action");

            var task = await _context.Tasks.FindAsync(taskId);
            if(task is null)
                return NotFound();

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
