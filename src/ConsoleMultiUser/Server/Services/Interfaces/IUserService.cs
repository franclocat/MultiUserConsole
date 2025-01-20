using Shared.DTO;

namespace Server.Services.Interfaces;

public interface IUserService
{
    Task<UserDTO> Add(UserDTO userDTO);
    Task<UserDTO?> ValidateCredentials(UserDTO userDto);
    Task<TokenDTO?> GenerateJwtIfCredentialsValid(UserDTO userDto);
}
