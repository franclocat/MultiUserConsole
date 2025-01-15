using Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Client.Services.Interfaces;

public interface IAuthService
{
    Task<ServiceResult> Register(UserDTO userDTO);
    Task<ServiceResult> Login(UserDTO userDto);
}
