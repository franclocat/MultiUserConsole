using Server.DataAccess.Model;
using Shared.DTO;

namespace Server.DataAccess;

public static class DTOMapper
{
    public static UserDTO ToDto(this User user)
    {
        return new UserDTO
        {
            Username = user.Username,
            Password = user.Password
        };
    }
}
