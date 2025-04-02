using System;

namespace Data.Entities;

public class MemberEntity : UserEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string JobTitle { get; set; }
    public string PhoneNumber { get; set; }
    public string ImageUrl { get; set; }
    public bool IsActive { get; set; } = true;
}
