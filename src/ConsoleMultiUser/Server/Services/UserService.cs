using Server.DataAccess;
using Server.DataAccess.Model;
using Server.Services.Interfaces;
using Shared.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Server.Services;

public class UserService : IUserService
{
    private readonly Db _db;
    private readonly IConfiguration _configuration;

    public UserService(Db db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
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

    public async Task<bool> ValidateCredentials(UserDTO userDto)
    {
        User? user = await _db.Users.FirstOrDefaultAsync(user => user.Username == userDto.Username);

        if (user != null) 
        {
            string hash = BuildHash(userDto.Password, user.Salt);

            if (hash.Equals(user.Hash))
            {
                return true;
            }
        }
        return false;
    }

    public async Task<string?> GenerateJwtIfCredentialsValid(UserDTO user)
    {
        try
        {
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            if (await ValidateCredentials(user))
            {
                JwtSecurityToken token = new JwtSecurityToken(
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);
                return new JwtSecurityTokenHandler().WriteToken(token);
            }

            throw new ApplicationException("Invalid Credentials.");
        }
        catch (Exception ex)
        {
            throw;
        }

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
