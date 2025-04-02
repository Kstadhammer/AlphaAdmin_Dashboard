using System;

namespace Business.Models;

public class Status
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Color { get; set; }
    public int Order { get; set; }
    public bool IsDefault { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
