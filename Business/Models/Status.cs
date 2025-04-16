using System;

namespace Business.Models;

public class Status
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Color { get; set; } = null!;
    public int Order { get; set; }
    public bool IsDefault { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
