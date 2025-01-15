using Microsoft.AspNetCore.Mvc;
using Server.Services;
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
}
