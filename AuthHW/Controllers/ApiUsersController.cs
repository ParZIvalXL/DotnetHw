using AuthHW.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthHW.Controllers
{
    [Route("api/users")]
    public class UsersController : Controller
    {
        private UsersService _usersService;
        
        public UsersController(UsersService usersService)
        {
            _usersService = usersService;
        }
        
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetUserMe()
        {
            try
            {
                var user = await _usersService.GetUserMe(User);
                if (user == null)
                    return NotFound(new { message = "Пользователь не найден" });

                return Ok(user);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "UserId claim not found" });
            }
            catch (Exception ex)
            {
                // можно логировать ex.Message
                return StatusCode(500, new { message = ex.Message });
            }
        }

        
        [Authorize]
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string tag)
        {
            var users = await _usersService.SearchUsers(tag);

            return Ok(users);
        }
        
        [Authorize]
        [HttpGet("debug-claims")]
        public IActionResult DebugClaims()
        {
            return Ok(new
            {
                IsAuthenticated = User.Identity?.IsAuthenticated,
                Claims = User.Claims.Select(c => new { c.Type, c.Value })
            });
        }

    }
    
    
}