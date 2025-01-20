using Shared.DTO;

namespace Client.Services.Interfaces;

public interface IAuthService
{
    Task<GenericServiceResult<UserDTO>> Register(UserDTO userDTO);
    Task<GenericServiceResult<UserDTO>> Login(UserDTO userDto);
    void SetAuthToken(string token);
    Task<ServiceResult> CheckAuthorization();
}
