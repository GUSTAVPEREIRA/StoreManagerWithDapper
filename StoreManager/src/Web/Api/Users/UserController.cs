using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Errors;
using Core.Users.Interfaces;
using Core.Users.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        [Authorize(Roles = "IsAdmin")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> InsertUser(UserRequest request)
        {
            try
            {
                var user = await _userService.InsertUserAsync(request);

                return CreatedAtAction(nameof(GetUser), new {id = user.Id}, user);
            }
            catch (EnvironmentVariableNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message, null, StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        [Route("Get/{id:int}")]
        [Authorize]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetUser(int id)
        {
            var user = await _userService.GetUserAsync(id);

            return Ok(user);
        }

        [HttpGet]
        [Route("List")]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<UserResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> GetUsers()
        {
            var users = await _userService.GetUsersAsync();

            return Ok(users);
        }

        [HttpPut]
        [Route("Update")]
        [Authorize(Roles = "IsAdmin")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateUser(UserUpdatedRequest request)
        {
            var user = await _userService.UpdatedUserAsync(request);

            return Ok(user);
        }
    }
}