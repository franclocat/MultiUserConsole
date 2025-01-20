using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Server.Services.Interfaces;
using Shared.DTO;
using System.IdentityModel.Tokens.Jwt;

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
            await _userService.Add(userDto);
            TokenDTO? userWithToken = await _userService.GenerateJwtIfCredentialsValid(userDto);
            userDto.TokenDto = userWithToken;
            return Ok(userDto);
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
            if (await _userService.ValidateCredentials(userDto))
            {
                TokenDTO? tokenDto = await _userService.GenerateJwtIfCredentialsValid(userDto);
                userDto.TokenDto = tokenDto;
                return Ok(userDto);
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

    [HttpGet("Checkauthorization")]
    [Authorize]
    public IActionResult CheckAuthorization()
    {
        return Ok();
    }
}
