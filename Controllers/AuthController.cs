using Facility_Management.DTOs;
using Facility_Management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace Facility_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JwtTokenService _tokenService;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            JwtTokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
        }

        public record RegisterDto(string UserName, string Email, string Password, string? FullName);
        public record LoginDto(string UserNameOrEmail, string Password);
        public record AssignRoleDto(string UserId, string Role);

        [HttpPost("register")]
      [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var user = new ApplicationUser { UserName = dto.UserName, Email = dto.Email, FullName = dto.FullName };
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);

            if (await _roleManager.RoleExistsAsync("User"))
                await _userManager.AddToRoleAsync(user, "User");

            var token = await _tokenService.CreateTokenAsync(user);
            return Ok(new { token, user = new { user.Id, user.UserName, user.Email } });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _userManager.FindByNameAsync(dto.UserNameOrEmail)
                       ?? await _userManager.FindByEmailAsync(dto.UserNameOrEmail);
            if (user == null) return Unauthorized("Invalid credentials.");

            var valid = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            if (!valid.Succeeded) return Unauthorized("Invalid credentials.");

            var token = await _tokenService.CreateTokenAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            return Ok(new { token, roles, user = new { user.Id, user.UserName, user.Email } });
        }


        
#if DEBUG


[AllowAnonymous]
    [HttpGet("dev-reset-redirect")]
    public async Task<IActionResult> DevResetRedirect([FromQuery] string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return BadRequest("Email is required");

        var user = await _userManager.FindByEmailAsync(email);
        // For security: do not reveal existence. If not found, still redirect to a generic page or 204.
        if (user == null)
            return NoContent(); // or Redirect("http://localhost:4200/forgot-password?sent=true");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encoded = Uri.EscapeDataString(token);

        // Use your actual Angular origin here (configurable)
        var resetUrl = $"http://localhost:4200/reset-password?email={email}&token={encoded}";

        // DEV: redirect (302) to Angular Reset page
        return Redirect(resetUrl);
    }
#endif




    [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
#if DEBUG
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null) return Ok(new { message = "Password reset link sent" });

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encoded = Uri.EscapeDataString(token);
            var resetUrl = $"http://localhost:4200/reset-password?email={dto.Email}&token={encoded}";

            return Ok(new
            {
                message = "Password reset link sent",
                resetUrl // 👈 copy this into the browser
            });
#else
    // Production behavior (do not return link)
    return Ok(new { message = "Password reset link sent" });
#endif
        }








        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null) return BadRequest(new { message = "Invalid request" });

            var rawToken = Uri.UnescapeDataString(dto.Token); // 👈 important
            var result = await _userManager.ResetPasswordAsync(user, rawToken, dto.NewPassword);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok("Password reset successful");
        }







        [HttpPost("assign-role")]
      [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> AssignRole(AssignRoleDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null) return NotFound("User not found.");
            if (!await _roleManager.RoleExistsAsync(dto.Role)) return BadRequest("Role does not exist.");
            var result = await _userManager.AddToRoleAsync(user, dto.Role);
            if (!result.Succeeded) return BadRequest(result.Errors);
            return Ok("Role assigned.");
        }


       [Authorize(Policy = "AdminOnly")]
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers([FromServices] UserManager<ApplicationUser> userManager)

        {
            // NOTE: userManager.Users may be IQueryable; we materialize it first.
            var users = userManager.Users.ToList();

            var list = new List<UserWithRolesDto>();
            foreach (var user in users)

            {
                var roles = await userManager.GetRolesAsync(user);
                list.Add(new UserWithRolesDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = roles
                });
            }

            return Ok(list);
        }



    }
}
