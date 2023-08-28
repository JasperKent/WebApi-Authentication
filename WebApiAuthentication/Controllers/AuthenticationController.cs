using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiAuthentication.Authentication;
using WebApiAuthentication.DataAccess.Entities;

namespace WebApiAuthentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<LibraryUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(UserManager<LibraryUser> userManager, IConfiguration configuration, ILogger<AuthenticationController> logger)
        {
            _userManager = userManager;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] RegistrationModel model)
        {
            _logger.LogInformation("Register called");

            var existingUser = await _userManager.FindByNameAsync(model.Username);

            if (existingUser != null)
                return Conflict("User already exists.");

            var newUser = new LibraryUser
            {
                Reviews = new List<BookReview>(),
                UserName = model.Username,
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(newUser, model.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("Register succeeded");

                return Ok("User successfully created");
            }
            else
                return StatusCode(StatusCodes.Status500InternalServerError,
                       $"Failed to create user: {string.Join(" ", result.Errors.Select(e => e.Description))}");
        }

        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            _logger.LogInformation("Login called");

            var user = await _userManager.FindByNameAsync(model.Username);

            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return Unauthorized();

            JwtSecurityToken token = GenerateJwt(model.Username);

            _logger.LogInformation("Login succeeded");

            return Ok(new LoginResponse
            {
                JwtToken = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo
            });
        }

        private JwtSecurityToken GenerateJwt(string username)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["JWT:Secret"] ?? throw new InvalidOperationException("Secret not configured")));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.UtcNow.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }
    }
}
