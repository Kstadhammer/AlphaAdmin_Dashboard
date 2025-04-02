using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Business.Models;

public class AddProjectForm
{
    [Required(ErrorMessage = "Project name is required")]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Client name is required")]
    [MaxLength(100)]
    public string ClientName { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public DateTime StartDate { get; set; } = DateTime.Today;

    [Required]
    public DateTime EndDate { get; set; } = DateTime.Today.AddMonths(1);

    public decimal Budget { get; set; }

    public List<string>? MemberIds { get; set; } = new List<string>();

    public bool IsActive { get; set; } = true;

    [DataType(DataType.Upload)]
    public IFormFile? ProjectImage { get; set; }
}
