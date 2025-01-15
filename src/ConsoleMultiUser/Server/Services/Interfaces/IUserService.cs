﻿using Shared.DTO;

namespace Server.Services.Interfaces;

public interface IUserService
{
    Task<UserDTO> Add(UserDTO userDTO);
    Task<bool> ValidateCredentials(UserDTO userDto);
}
