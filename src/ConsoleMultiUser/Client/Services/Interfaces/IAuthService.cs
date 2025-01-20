using Shared.DTO;

namespace Client.Services.Interfaces;

public interface IAuthService
{
    Task<ServiceResult> Register(UserDTO userDTO);
    //Task<ServiceResult> Login(UserDTO userDto);
    Task<GenericServiceResult<string>> Login(UserDTO userDto);
    void SetAuthToken(string token);
    Task<ServiceResult> CheckAuthorization();
}
