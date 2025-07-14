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
        ICommentService _commentService;
        
        int? userId;
        int _userId => userId ??= Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

        int[] _userRoles = [1, 2, 3];
        int[] _adminRoles = [1, 2];
        int[] _ownerRoles = [1];
        public CommentsController(ICommentService commentService, IAuthorizationService authorizationService) 
        {
            _commentService = commentService;
            _authorizationService = authorizationService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAsync(int taskId, CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByTaskIdAsync(taskId, _userId, _userRoles, cancellationToken);
            if(!isAuthorized)
                return Unauthorized("You havent access to this action");

            var comments = _commentService.GetAsync(taskId, cancellationToken);
            return comments is null ? NotFound() : Ok(comments);
        }
        [HttpPost]
        public async Task<IActionResult> PostAsync(CommentDTO comment, CancellationToken cancellationToken)
        {
            bool participantId = await _authorizationService.AccessByTaskIdAsync(comment.TaskId, _userId, _adminRoles, cancellationToken);
            if (participantId is false)
                return Unauthorized("You havent access to this action");

            var newComment = _commentService.PostAsync(comment, cancellationToken);
            return Ok(newComment.Id);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateAsync(CommentDTO comment,CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByCommentIdAsync(comment.Id, _userId, cancellationToken);
            if(!isAuthorized)
                return Unauthorized("You havent access to this action");

            var newComment = _commentService.UpdateAsync(comment, cancellationToken);
            return newComment is null ? NotFound() : NoContent();
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(int commentId,CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByCommentIdAsync(commentId, _userId, cancellationToken);
            if (!isAuthorized)
                return Unauthorized("You havent access to this action");

            try
            {
                await _commentService.DeleteAsync(commentId, cancellationToken);
                return NoContent();
            }
            catch(KeyNotFoundException) { return NotFound(); }
            catch(Exception) { return StatusCode(500, "Internal server error"); }
        }
    }
}
