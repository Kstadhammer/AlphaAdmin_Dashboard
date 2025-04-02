using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models;

public class Project
{
    [Key]
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string ClientName { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Budget { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; }
    public List<Member> Members { get; set; } = new List<Member>();

    public Client Client { get; set; } = null!;

    public User User { get; set; } = null!;

    public Status Status { get; set; } = null!;
}
