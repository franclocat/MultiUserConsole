using Server.DataAccess;
using Server.DataAccess.Model;
using Server.Services.Interfaces;
using Shared.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

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
        //Safe password handling
        string salt = Convert.ToBase64String(RandomNumberGenerator.GetBytes(128));
        string userHash = BuildHash(userDTO.Password, salt);

        User newUser = new User()
        {
            Username = userDTO.Username,
            Hash = userHash,
            Salt = salt
        };

        if (_db.Users.Any(user => user.Username == userDTO.Username))
        {
            throw new Exception("A user with this username already exists.");
        }

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

    private static string BuildHash(string password, string salt)
    {
        return Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: Convert.FromBase64String(salt),
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8));
    }
}
