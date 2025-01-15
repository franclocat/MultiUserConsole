using Microsoft.AspNetCore.Mvc;
using Server.Services.Interfaces;
using Shared.DTO;

namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("Register")]
    public async Task<ActionResult<UserDTO>> Register(UserDTO userDto)
    {
        try
        {
            return await _userService.Add(userDto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("Login")]
    public async Task<ActionResult<string>> Login(UserDTO userDto)
    {
        try
        {
            if (await _userService.ValidateCredentials(userDto))
            {
                string? token = await _userService.GenerateJwtIfCredentialsValid(userDto);
                return Ok(token);
            }
            return Unauthorized("Username or password is wrong");
        }
        catch (ApplicationException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest($"An unhandled exception ocurred: {ex.Message}");
        }
    }
}
