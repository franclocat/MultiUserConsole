using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Server.DataAccess;
using Server.DataAccess.Model;
using Server.Services.Interfaces;
using Shared.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

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
            throw new ApplicationException("A user with this username already exists.");
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

    public async Task<UserDTO?> ValidateCredentials(UserDTO userDto)
    {
        User? user = await _db.Users.FirstOrDefaultAsync(user => user.Username == userDto.Username);

        if (user != null) 
        {
            string hash = BuildHash(userDto.Password, user.Salt);

            if (hash.Equals(user.Hash))
            {
                return new UserDTO
                {
                    Id = user.Id,
                    Username = user.Username,
                    Password = user.Password
                };
            }
        }
        return null;
    }

    public async Task<TokenDTO?> GenerateJwtIfCredentialsValid(UserDTO userDto)
    {
        try
        {
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //get user and roles from db
            User? userFromDb = await _db.Users
                .Include(user => user.Roles)
                .FirstOrDefaultAsync(user => user.Id == userDto.Id);

            JwtSecurityToken token = new JwtSecurityToken(
            expires: DateTime.Now.AddMinutes(int.Parse(_configuration["Jwt:ExpirationInMinutes"])),
            signingCredentials: credentials,
            //add user roles as claims in the token
            claims: userFromDb.Roles.Select(role => new Claim(ClaimTypes.Role, role.Name))
            );


            //return new JwtSecurityTokenHandler().WriteToken(token); //gives a string as a result
            return new TokenDTO
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    ExpiryDate = token.ValidTo.ToLocalTime(),
                    Roles = token.Claims.Select(role =>  role.Value.ToUpper()).ToList()
                };
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
