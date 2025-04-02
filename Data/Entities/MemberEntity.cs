using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Data.Entities;

public class MemberEntity : IdentityUser // Inherit directly from IdentityUser
{
    // Properties originally from UserEntity (making FirstName/LastName non-nullable)
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? JobTitle { get; set; } // Kept nullable as in UserEntity
    public virtual ICollection<ProjectEntity> Projects { get; set; } = new List<ProjectEntity>(); // Initialize collection

    // Properties originally in MemberEntity
    // public string PhoneNumber { get; set; } // Already part of IdentityUser
    public string? ImageUrl { get; set; } // Made nullable for flexibility
    public bool IsActive { get; set; } = true;

    // Note: Email, PhoneNumber are part of IdentityUser base class
}
