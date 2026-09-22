using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Data;
using TaskManagement.API.DTOS;
using TaskManagement.API.Models;
using TaskManagement.API.Services;

namespace TaskManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwtService;

        public AuthController(
            AppDbContext context,
            JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(
            RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var email = request.Email.Trim().ToLower();

            var existingUser =
                await _context.Users
                    .FirstOrDefaultAsync(
                        x => x.Email == email);

            if (existingUser != null)
            {
                return Conflict(new
                {
                    message = "Email is already registered."
                });
            }

            var allowedRoles =
                new[] { "Admin", "Manager", "User" };

            if (!allowedRoles.Contains(request.Role))
            {
                return BadRequest(new
                {
                    message =
                        "Invalid role. Allowed roles: Admin, Manager, User."
                });
            }

            var passwordHash =
                BCrypt.Net.BCrypt.HashPassword(
                    request.Password);

            var user = new User
            {
                FullName = request.FullName.Trim(),

                Email = email,

                PasswordHash = passwordHash,

                Role = request.Role
            };

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Registration successful.",
                userId = user.Id,
                fullName = user.FullName,
                email = user.Email,
                role = user.Role
            });
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(
            LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var email = request.Email.Trim().ToLower();

            var user =
                await _context.Users
                    .FirstOrDefaultAsync(
                        x => x.Email == email);

            if (user == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });
            }

            var passwordValid =
                BCrypt.Net.BCrypt.Verify(
                    request.Password,
                    user.PasswordHash);

            if (!passwordValid)
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });
            }

            var jwt =
                _jwtService.GenerateToken(user);

            var response = new AuthResponse
            {
                Token = jwt.Token,

                UserId = user.Id,

                FullName = user.FullName,

                Email = user.Email,

                Role = user.Role,

                ExpiresAt = jwt.ExpiresAt
            };

            return Ok(response);
        }
    }
}