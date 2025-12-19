using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskTracker.Api.Models;

namespace TaskTracker.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] LoginModel model)
        {
            if (model.Username == null || model.Password == null)
                return BadRequest("Username and password are required.");

            var user = new IdentityUser { UserName = model.Username };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
                return Ok();

            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (model.Username == null || model.Password == null)
                return BadRequest("Username and password are required.");

            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
                return Unauthorized("Invalid username or password.");

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!signInResult.Succeeded)
                return Unauthorized("Invalid username or password.");

            // Build claims
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // JWT key
            var key = Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"] 
                ?? throw new InvalidOperationException("JWT Key is missing")
            );

            if (key.Length < 32)
                throw new InvalidOperationException("JWT Key must be at least 32 bytes long.");

            var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }
    }
}
