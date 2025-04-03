using Business.Models;
using Domain.Models;

namespace Business.Interfaces;

public interface IMemberService
{
    Task<Member> GetCurrentUserAsync(string userId);
    Task<List<Member>> GetAllMembers();
    Task<EditMemberForm?> GetMemberForEditAsync(string id);
    Task<bool> EditMemberAsync(EditMemberForm form);
    Task<bool> DeleteMemberAsync(string id);
}
