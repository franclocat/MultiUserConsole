using Server.DataAccess;
using Server.DataAccess.Model;
using Server.Services.Interfaces;
using Shared.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Server.Services;

public class UserService : IUserService
{
    private readonly Db _db;

    public UserService(Db db)
    {
        _db = db;
    }

    public async Task<UserDTO> Add(UserDTO userDTO)
    {
        User newUser = new User()
        {
            Username = userDTO.Username,
            Password = userDTO.Password,
        };

        try
        {
            await _db.Users.AddAsync(newUser);
            await _db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw;
        }

        userDTO.Id = newUser.Id;
        return userDTO;
    }
}
