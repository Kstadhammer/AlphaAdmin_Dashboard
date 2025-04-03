using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class ProjectEntity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; }
    public string ClientName { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal? Budget { get; set; }
    public string ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; }
    public string GradientCss { get; set; } = string.Empty; // Store CSS for background gradient

    [NotMapped]
    public virtual ICollection<MemberEntity> Members { get; set; } = new List<MemberEntity>();

    [ForeignKey(nameof(Client))]
    public string ClientId { get; set; } = null!;
    public virtual ClientEntity Client { get; set; } = null!;

    [ForeignKey(nameof(User))]
    public string UserId { get; set; } = null!;
    public MemberEntity User { get; set; } = null!;

    [ForeignKey(nameof(Status))]
    public string StatusId { get; set; } = null!;
    public virtual StatusEntity Status { get; set; } = null!;
}
