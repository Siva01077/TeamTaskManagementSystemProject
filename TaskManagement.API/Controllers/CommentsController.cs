using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Data;
using TaskManagement.API.Models;

namespace TaskManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CommentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CommentsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/comments/task/5
        [HttpGet("task/{taskId}")]
        public async Task<IActionResult> GetComments(
            int taskId)
        {
            var task = await _context.Tasks
                .FirstOrDefaultAsync(x => x.Id == taskId);

            if (task == null)
            {
                return NotFound(new
                {
                    message = "Task not found."
                });
            }

            var comments = await _context.Comments
                .Include(x => x.User)
                .Where(x => x.TaskId == taskId)
                .OrderBy(x => x.CreatedAt)
                .Select(x => new
                {
                    x.Id,
                    x.Content,
                    x.CreatedAt,

                    User = new
                    {
                        x.User.Id,
                        x.User.FullName,
                        x.User.Role
                    }
                })
                .ToListAsync();

            return Ok(comments);
        }

        // POST: api/comments
        [HttpPost]
        public async Task<IActionResult> AddComment(
            CreateCommentRequest request)
        {
            var userId =
                int.Parse(
                    User.FindFirstValue(
                        ClaimTypes.NameIdentifier)!);

            var user = await _context.Users
                .FindAsync(userId);

            var task = await _context.Tasks
                .FirstOrDefaultAsync(
                    x => x.Id == request.TaskId);

            if (task == null)
            {
                return NotFound(new
                {
                    message = "Task not found."
                });
            }

            if (user == null)
            {
                return Unauthorized();
            }

            // User can comment only on assigned task
            if (user.Role == "User" &&
                task.AssignedToUserId != userId)
            {
                return Forbid();
            }

            // Manager can comment only on team task
            if (user.Role == "Manager" &&
                task.TeamId != user.TeamId)
            {
                return Forbid();
            }

            if (string.IsNullOrWhiteSpace(
                request.Content))
            {
                return BadRequest(new
                {
                    message = "Comment cannot be empty."
                });
            }

            var comment = new Comment
            {
                TaskId = request.TaskId,

                UserId = userId,

                Content = request.Content.Trim(),

                CreatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(comment);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Comment added successfully.",
                comment.Id,
                comment.Content,
                comment.CreatedAt
            });
        }
    }

    public class CreateCommentRequest
    {
        public int TaskId { get; set; }

        public string Content { get; set; } = string.Empty;
    }
}