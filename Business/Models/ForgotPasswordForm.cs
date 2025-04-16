using System.ComponentModel.DataAnnotations;

namespace Business.Models;

public class ForgotPasswordForm
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = string.Empty;
}
