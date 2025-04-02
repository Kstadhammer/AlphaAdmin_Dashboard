using System;

namespace Business.Models;

public class ClientListItem
{
    public string Id { get; set; }
    public string ClientName { get; set; }
    public string Email { get; set; }
    public string Location { get; set; }
    public string Phone { get; set; }
    public string ImageUrl { get; set; }
    public bool IsActive { get; set; }
}
