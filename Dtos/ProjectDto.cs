namespace Dtos;

public class ProjectDto
{
    public string Id { get; set; } = null!;
    public string ProjectName { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime Created { get; set; }

    // Flattened/Simplified related data
    public ClientDto? Client { get; set; } // Include basic client info
    public StatusDto? Status { get; set; } // Include basic status info
    public UserDto? User { get; set; } // Include basic user info

    // Add other fields as needed, e.g., ImageUrl
}
