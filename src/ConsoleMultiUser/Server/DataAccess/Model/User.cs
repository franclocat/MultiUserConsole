using System.ComponentModel.DataAnnotations.Schema;

namespace Server.DataAccess.Model;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    [NotMapped]
    public string Password { get; set; } //Can be deleted but wont be added to the DB because of the NotMapped Attribute
    public string Salt { get; set; } = string.Empty;
    public string Hash { get; set; } = string.Empty;
    public ICollection<Role> Roles { get; set; }
}
