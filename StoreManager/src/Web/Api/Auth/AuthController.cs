using System.Threading.Tasks;
using Application.Users;
using Core.Auth.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private UserService _userService;

        public AuthController(UserService userService)
        {
            _userService = userService;
        }


        [HttpPost]
        [Route("AuthLogin")]
        public async Task<ActionResult> InsertUser(AuthLoginRequest request)
        {
            var user = await _userService.GetUserByEmailAndPasswordAsync(request);

            return Ok(user);
        }
    }
}