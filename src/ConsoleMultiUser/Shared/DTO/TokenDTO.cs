﻿namespace Shared.DTO;

public class TokenDTO
{
    public string Token { get; set; }
    public DateTime ExpiryDate { get; set; }
    public ICollection<string> Roles { get; set; }
}
