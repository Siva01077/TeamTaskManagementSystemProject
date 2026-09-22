using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Models;
using TaskManagement.API.Data;

namespace TaskManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/users
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .Include(x => x.Team)
                .Select(x => new
                {
                    x.Id,
                    x.FullName,
                    x.Email,
                    x.Role,
                    TeamId = x.TeamId,
                    TeamName = x.Team != null
                        ? x.Team.Name
                        : null
                })
                .ToListAsync();

            return Ok(users);
        }

        // GET: api/users/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _context.Users
                .Include(x => x.Team)
                .Where(x => x.Id == id)
                .Select(x => new
                {
                    x.Id,
                    x.FullName,
                    x.Email,
                    x.Role,
                    TeamId = x.TeamId,
                    TeamName = x.Team != null
                        ? x.Team.Name
                        : null
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound(new
                {
                    message = "User not found."
                });
            }

            return Ok(user);
        }

        // PUT: api/users/5/role
        [HttpPut("{id}/role")]
        public async Task<IActionResult> ChangeRole(
            int id,
            [FromBody] ChangeRoleRequest request)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound(new
                {
                    message = "User not found."
                });
            }

            var validRoles =
                new[] { "Admin", "Manager", "User" };

            if (!validRoles.Contains(request.Role))
            {
                return BadRequest(new
                {
                    message = "Invalid role."
                });
            }

            user.Role = request.Role;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "User role updated successfully."
            });
        }

        // DELETE: api/users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound(new
                {
                    message = "User not found."
                });
            }

            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "User deleted successfully."
            });
        }
    }

    public class ChangeRoleRequest
    {
        public string Role { get; set; } = string.Empty;
    }
}