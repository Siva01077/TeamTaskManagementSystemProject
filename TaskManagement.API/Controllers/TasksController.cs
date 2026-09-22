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
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TasksController(AppDbContext context)
        {
            _context = context;
        }

        // ADMIN + MANAGER
        // POST: api/tasks
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> CreateTask(
            CreateTaskRequest request)
        {
            var user = await _context.Users
                .FindAsync(request.AssignedToUserId);

            if (user == null)
            {
                return BadRequest(new
                {
                    message = "Assigned user does not exist."
                });
            }

            var currentUserId = GetCurrentUserId();

            var currentUser = await _context.Users
                .FindAsync(currentUserId);

            if (currentUser == null)
            {
                return Unauthorized();
            }

            // Manager can only assign tasks
            // to users in their own team.
            if (currentUser.Role == "Manager")
            {
                if (currentUser.TeamId == null ||
                    user.TeamId != currentUser.TeamId)
                {
                    return Forbid();
                }
            }

            var task = new TaskItem
            {
                Title = request.Title,

                Description = request.Description,

                Status = "To Do",

                Priority = request.Priority,

                Deadline = request.Deadline,

                AssignedToUserId =
                    request.AssignedToUserId,

                TeamId = user.TeamId,

                CreatedByUserId = currentUserId,

                CreatedAt = DateTime.UtcNow
            };

            _context.Tasks.Add(task);

            await _context.SaveChangesAsync();

            // Create assignment notification
            var notification = new Notification
            {
                UserId = user.Id,

                TaskId = task.Id,

                Type = "TaskAssigned",

                Message =
                    $"You have been assigned task: {task.Title}",

                IsRead = false
            };

            _context.Notifications.Add(notification);

            await _context.SaveChangesAsync();

            return Ok(task);
        }

        // GET: api/tasks
        [HttpGet]
        public async Task<IActionResult> GetTasks(
            string? status,
            string? priority,
            DateTime? deadline)
        {
            var userId = GetCurrentUserId();

            var user = await _context.Users
                .FindAsync(userId);

            if (user == null)
            {
                return Unauthorized();
            }

            var query = _context.Tasks
                .Include(x => x.AssignedToUser)
                .AsQueryable();

            // USER:
            // Only assigned tasks
            if (user.Role == "User")
            {
                query = query.Where(x =>
                    x.AssignedToUserId == userId);
            }

            // MANAGER:
            // Tasks belonging to manager's team
            if (user.Role == "Manager")
            {
                query = query.Where(x =>
                    x.TeamId == user.TeamId);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(x =>
                    x.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(priority))
            {
                query = query.Where(x =>
                    x.Priority == priority);
            }

            if (deadline.HasValue)
            {
                query = query.Where(x =>
                    x.Deadline.Date ==
                    deadline.Value.Date);
            }

            var tasks = await query
                .OrderBy(x => x.Deadline)
                .Select(x => new
                {
                    x.Id,
                    x.Title,
                    x.Description,
                    x.Status,
                    x.Priority,
                    x.Deadline,
                    x.CreatedAt,

                    AssignedTo = new
                    {
                        x.AssignedToUser.Id,
                        x.AssignedToUser.FullName,
                        x.AssignedToUser.Email
                    }
                })
                .ToListAsync();

            return Ok(tasks);
        }

        // GET: api/tasks/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask(int id)
        {
            var userId = GetCurrentUserId();

            var user = await _context.Users
                .FindAsync(userId);

            if (user == null)
            {
                return Unauthorized();
            }

            var task = await _context.Tasks
                .Include(x => x.AssignedToUser)
                .Include(x => x.Comments)
                    .ThenInclude(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (task == null)
            {
                return NotFound();
            }

            // User can only view assigned task
            if (user.Role == "User" &&
                task.AssignedToUserId != userId)
            {
                return Forbid();
            }

            // Manager can only view team tasks
            if (user.Role == "Manager" &&
                task.TeamId != user.TeamId)
            {
                return Forbid();
            }

            return Ok(task);
        }

        // Update status
        // PUT: api/tasks/5/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(
            int id,
            UpdateStatusRequest request)
        {
            var userId = GetCurrentUserId();

            var user = await _context.Users
                .FindAsync(userId);

            if (user == null)
            {
                return Unauthorized();
            }

            var task = await _context.Tasks
                .FirstOrDefaultAsync(x => x.Id == id);

            if (task == null)
            {
                return NotFound();
            }

            // User can update only their own task
            if (user.Role == "User" &&
                task.AssignedToUserId != userId)
            {
                return Forbid();
            }

            // Manager can update team tasks
            if (user.Role == "Manager" &&
                task.TeamId != user.TeamId)
            {
                return Forbid();
            }

            var validStatuses =
                new[]
                {
                    "To Do",
                    "In Progress",
                    "Done"
                };

            if (!validStatuses.Contains(request.Status))
            {
                return BadRequest(new
                {
                    message =
                        "Invalid status."
                });
            }

            task.Status = request.Status;

            await _context.SaveChangesAsync();

            // Notification for task creator
            if (task.CreatedByUserId != userId)
            {
                _context.Notifications.Add(
                    new Notification
                    {
                        UserId =
                            task.CreatedByUserId,

                        TaskId = task.Id,

                        Type = "TaskStatusUpdated",

                        Message =
                            $"Task '{task.Title}' status changed to {task.Status}",

                        IsRead = false
                    });

                await _context.SaveChangesAsync();
            }

            return Ok(new
            {
                message =
                    "Task status updated successfully.",
                status = task.Status
            });
        }

        private int GetCurrentUserId()
        {
            var userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            return int.Parse(userId!);
        }
    }

    public class CreateTaskRequest
    {
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Priority { get; set; } = "Medium";

        public DateTime Deadline { get; set; }

        public int AssignedToUserId { get; set; }
    }

    public class UpdateStatusRequest
    {
        public string Status { get; set; } = string.Empty;
    }
}