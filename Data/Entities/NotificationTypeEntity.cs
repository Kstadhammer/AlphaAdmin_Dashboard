using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class NotificationTypeEntity
{
    [Key]
    public int Id { get; set; }
    public string NotificationType { get; set; } = null!;
}
