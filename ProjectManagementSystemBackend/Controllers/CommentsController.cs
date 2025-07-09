using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Interfaces;
using ProjectManagementSystemBackend.Models;
using ProjectManagementSystemBackend.Models.DTO;
using ProjectManagementSystemBackend.Services;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using IAuthorizationService = ProjectManagementSystemBackend.Interfaces.IAuthorizationService;

namespace ProjectManagementSystemBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CommentsController : ControllerBase
    {
        ApplicationContext _context;
        IAuthorizationService _authorizationService;
        
        int? userId;
        int _userId => userId ??= Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

        int[] _userRoles = [1, 2, 3];
        int[] _adminRoles = [1, 2];
        int[] _ownerRoles = [1];
        public CommentsController(ApplicationContext context, IAuthorizationService authorizationService) 
        {
            _context = context;
            _authorizationService = authorizationService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAsync(int taskId, CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByTaskIdAsync(taskId, _userId, _userRoles, cancellationToken);
            if(!isAuthorized)
                return Unauthorized("You havent access to this action");

            var comments  = await _context.TaskComments
                .AsNoTracking()
                .Where(c => c.TaskId == taskId)
                .ProjectToType<CommentDTO>()
                .ToListAsync();
            return comments is null ? NotFound() : Ok(comments);
        }
        [HttpPost]
        public async Task<IActionResult> PostAsync(CommentDTO comment, CancellationToken cancellationToken)
        {
            int participantId = await _authorizationService.AccessAndParticipantByTaskIdAsync(comment.TaskId, _userId, _adminRoles, cancellationToken);
            if (participantId == 0)
                return Unauthorized("You havent access to this action");

            TaskComment newComment = comment.Adapt<TaskComment>();
            await _context.TaskComments.AddAsync(newComment);
            await _context.SaveChangesAsync();

            return Ok(newComment.Id);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateAsync(CommentDTO comment,CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByCommentIdAsync(comment.Id, _userId, cancellationToken);
            if(!isAuthorized)
                return Unauthorized("You havent access to this action");

            var updatedComment = await _context.TaskComments.FindAsync(comment.Id);
            if (updatedComment is null)
                return NotFound();
            updatedComment.Message = comment.Message;
            await _context.SaveChangesAsync();
            
            return NoContent();
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(int commentId,CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByCommentIdAsync(commentId, _userId, cancellationToken);
            if (!isAuthorized)
                return Unauthorized("You havent access to this action");

            var comment = await _context.TaskComments.FindAsync(commentId);
            if(comment is null)
                return NotFound();
            _context.TaskComments.Remove(comment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
