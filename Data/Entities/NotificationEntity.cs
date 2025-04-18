using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class NotificationEntity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TargetGroup { get; set; } = null!;
    public string NotificationType { get; set; } = null!;
    public string Icon { get; set; } = null!;
    public string Message { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
