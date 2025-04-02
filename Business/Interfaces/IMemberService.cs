using Business.Models;
using Domain.Models;

namespace Business.Interfaces;

public interface IMemberService
{
    Task<Member> GetCurrentUserAsync(string userId);
    Task<List<Member>> GetAllMembers();
}
