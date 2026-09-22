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
    public class TeamsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TeamsController(AppDbContext context)
        {
            _context = context;
        }

        // ADMIN: Create team
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTeam(
            [FromBody] CreateTeamRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest(new
                {
                    message = "Team name is required."
                });
            }

            var exists = await _context.Teams
                .AnyAsync(x => x.Name == request.Name);

            if (exists)
            {
                return Conflict(new
                {
                    message = "Team already exists."
                });
            }

            var team = new Team
            {
                Name = request.Name.Trim()
            };

            _context.Teams.Add(team);

            await _context.SaveChangesAsync();

            return Ok(team);
        }

        // ADMIN + MANAGER: View teams
        [HttpGet]
        public async Task<IActionResult> GetTeams()
        {
            var teams = await _context.Teams
                .Include(x => x.Users)
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    UserCount = x.Users.Count
                })
                .ToListAsync();

            return Ok(teams);
        }

        // ADMIN + MANAGER: Get team members
        [HttpGet("{id}/members")]
        public async Task<IActionResult> GetMembers(int id)
        {
            var team = await _context.Teams
                .Include(x => x.Users)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (team == null)
            {
                return NotFound(new
                {
                    message = "Team not found."
                });
            }

            return Ok(team.Users.Select(x => new
            {
                x.Id,
                x.FullName,
                x.Email,
                x.Role
            }));
        }

        // ADMIN + MANAGER: Assign user to team
        [HttpPost("{teamId}/members/{userId}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> AddMember(
            int teamId,
            int userId)
        {
            var team = await _context.Teams.FindAsync(teamId);

            if (team == null)
            {
                return NotFound(new
                {
                    message = "Team not found."
                });
            }

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound(new
                {
                    message = "User not found."
                });
            }

            user.TeamId = teamId;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "User assigned to team successfully."
            });
        }

        // ADMIN + MANAGER: Remove user from team
        [HttpDelete("{teamId}/members/{userId}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> RemoveMember(
            int teamId,
            int userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x =>
                    x.Id == userId &&
                    x.TeamId == teamId);

            if (user == null)
            {
                return NotFound(new
                {
                    message = "User is not a member of this team."
                });
            }

            user.TeamId = null;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "User removed from team."
            });
        }
    }

    public class CreateTeamRequest
    {
        public string Name { get; set; } = string.Empty;
    }
}