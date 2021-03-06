using System;
using System.Threading.Tasks;
using Core.Auth.Interfaces;
using Core.Auth.Models;
using Core.Users.Interfaces;
using Core.Users.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Auth;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IJwtService _jwtService;

    public AuthController(IUserService userService, IJwtService jwtService)
    {
        _userService = userService;
        _jwtService = jwtService;
    }


    [HttpPost]
    [Route("AuthLogin")]
    [ProducesResponseType(typeof(BearerTokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [AllowAnonymous]
    public async Task<ActionResult> AuthLogin(AuthLoginRequest request)
    {
        try
        {
            var userResponse = await _userService.GetUserByEmailAndPasswordAsync(request);
            var bearerToken = _jwtService.GenerateToken(userResponse);
            return Ok(bearerToken);
        }
        catch (Exception)
        {
            return Unauthorized("Invalid password or email");
        }
    }

    [HttpPost]
    [Route("RestartDefaultUser")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> RestartDefaultUser()
    {
        try
        {
            var user = await _userService.CreateOrUpdateUserDefault();

            return Ok(user);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}