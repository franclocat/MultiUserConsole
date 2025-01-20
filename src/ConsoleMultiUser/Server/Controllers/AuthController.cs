using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.DataAccess.Model;
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
            UserDTO createdUser = await _userService.Add(userDto);
            TokenDTO? userWithToken = await _userService.GenerateJwtIfCredentialsValid(createdUser);
            createdUser.TokenDto = userWithToken;
            return Ok(createdUser);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("Login")]
    public async Task<ActionResult<UserDTO>> Login(UserDTO userDto)
    {
        try
        {
            UserDTO? validatedUser = await _userService.ValidateCredentials(userDto);
            if (validatedUser != null)
            {
                TokenDTO? tokenDto = await _userService.GenerateJwtIfCredentialsValid(validatedUser);
                validatedUser.TokenDto = tokenDto;
                return Ok(validatedUser);
            }
            return Unauthorized("Username or password is wrong");
        }
        catch (Exception ex)
        {
            return BadRequest($"An unhandled exception ocurred: {ex.Message}");
        }
    }

    [HttpGet("Checkauthorization")]
    [Authorize]
    [Authorize(Roles = $"{Policies.Admin},{Policies.Customer}")]
    public IActionResult CheckAuthorization()
    {
        return Ok();
    }
}
