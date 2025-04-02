using Business.Models;
using Domain.Models;

namespace Business.Interfaces;

public interface IUserService
{
    Task<UserResult> GetUsersAsync();
    Task<UserResult> AddUserToRole(string userId, string roleName);
    Task<UserResult> CreateUserAsync(SignUpFormData formData, string roleName);
    Task<UserResult> GetUserByIdAsync(string userId);
}
