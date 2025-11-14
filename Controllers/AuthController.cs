using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LogiTrack.Controllers
{
    public class AuthenticationRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class AssignRoleRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    /* API controller for user authentication and role management */
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _config;

        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
        }

        // POST /api/auth/register
        /* Registers a new user */
        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] AuthenticationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Username) || string.IsNullOrWhiteSpace(request?.Password))
                return BadRequest("Username and password are required.");

            var user = new ApplicationUser { UserName = request.Username };
            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
                return Ok(new { message = "User registered successfully." });

            return Conflict(new { errors = result.Errors.Select(e => e.Description) });
        }

        // POST /api/auth/login
        /* Logs in a user and returns a JWT token */
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] AuthenticationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Username) || string.IsNullOrWhiteSpace(request?.Password))
                return BadRequest("Username and password are required.");

            var result = await _signInManager.PasswordSignInAsync(request.Username, request.Password, false, false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(request.Username);
                if (user is null) {
                    return Unauthorized(new { message = "Invalid username or password." });
                }

                var token = GenerateJwtToken(user);
                return Ok(new { token });
            }

            return Unauthorized(new { message = "Invalid username or password." });
        }

        private string GenerateJwtToken(IdentityUser user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"] ?? "your-secret-key-here"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName is null ? string.Empty : user.UserName)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // POST /api/auth/assign-role
        /* Assigns a role to a user */
        [HttpPost("assign-role")]
        public async Task<ActionResult> AssignRole([FromBody] AssignRoleRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Username) || string.IsNullOrWhiteSpace(request?.Role))
                return BadRequest("Username and role are required.");

            var user = await _userManager.FindByNameAsync(request.Username);
            
            if (user is null)
                return NotFound(new { message = "User not found." });

            var result = await _userManager.AddToRoleAsync(user, request.Role);

            if (result.Succeeded)
                return Ok(new { message = $"Role '{request.Role}' assigned to user '{request.Username}' successfully." });

            return Conflict(new { errors = result.Errors.Select(e => e.Description) });
        }
    }
}