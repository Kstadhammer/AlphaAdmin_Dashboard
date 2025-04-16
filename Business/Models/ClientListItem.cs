using System;

namespace Business.Models;

public class ClientListItem
{
    public string Id { get; set; } = null!;
    public string ClientName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Location { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string ImageUrl { get; set; } = null!;
    public bool IsActive { get; set; }
}
