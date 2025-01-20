using System.ComponentModel.DataAnnotations;

namespace Shared.DTO;

public class UserDTO
{
    public int Id { get; set; }
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
    public TokenDTO? TokenDto { get; set; }
    public List<RoleDTO>? Roles { get; set; }
}
