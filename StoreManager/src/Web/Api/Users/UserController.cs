using System.Threading.Tasks;
using Core.Users.Interfaces;
using Core.Users.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Users
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Route("Create")]
        public async Task<ActionResult> InsertUser(UserRequest request)
        {
            var user = await _userService.InsertUserAsync(request);

            return CreatedAtAction(nameof(GetUser), new {id = user.Id}, user);
        }

        [HttpGet]
        [Route("Get/{id:int}")]
        public async Task<ActionResult> GetUser(int id)
        {
            var user = await _userService.GetUserAsync(id);

            return Ok(user);
        }

        [HttpGet]
        [Route("List")]
        public async Task<ActionResult> GetUsers()
        {
            var users = await _userService.GetUsersAsync();

            return Ok(users);
        }

        [HttpPut]
        [Route("Update")]
        public async Task<ActionResult> UpdateUser(UserUpdatedRequest request)
        {
            var user = await _userService.UpdatedUserAsync(request);

            return Ok(user);
        }
    }
}