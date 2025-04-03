using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class ProjectMemberEntity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [ForeignKey(nameof(Project))]
    public string ProjectId { get; set; } = null!;
    public virtual ProjectEntity Project { get; set; } = null!;
    
    [ForeignKey(nameof(Member))]
    public string MemberId { get; set; } = null!;
    public virtual MemberEntity Member { get; set; } = null!;
}
