using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Movies.Domain.Interfaces;
using Movies.Domain.Models;

namespace MoviesApp.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public UsersController(UserManager<ApplicationUser> userManager)
        {
           _userManager = userManager;
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteUser(string? userId)
        {
            var user = await  _userManager.FindByIdAsync(userId);

            if(user == null)
                return NotFound();

            var result = await  _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                throw new Exception();

            return Ok();
        }
    }
}
