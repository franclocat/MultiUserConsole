using System.ComponentModel.DataAnnotations;

namespace Server.DataAccess.Model;

public class Product
{
    public int Id { get; set; }
    [Required]
    [MaxLength(50)]
    public string Title { get; set; }

    [MaxLength(100)]
    public string Description { get; set; }
}
