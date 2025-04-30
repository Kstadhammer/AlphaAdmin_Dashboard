using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Business.Forms;

public class EditProjectForm
{
    public string Id { get; set; }

    [Required(ErrorMessage = "Project name is required")]
    [MaxLength(100)]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Client name is required")]
    [MaxLength(100)]
    public string ClientName { get; set; } = null!;

    public string Description { get; set; } = null!;

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    public decimal Budget { get; set; }

    public bool IsActive { get; set; }

    public List<string> MemberIds { get; set; } = new List<string>();

    [Required(ErrorMessage = "Status is required")]
    public string StatusId { get; set; } = string.Empty;

    [DataType(DataType.Upload)]
    public IFormFile? ProjectImage { get; set; }
}
