namespace Domain.Models;

public class Member
{
    public string Id { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string JobTitle { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public bool IsAdmin { get; set; } = false;
}
