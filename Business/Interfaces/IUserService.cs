using Business.Models;
using Domain.Models;

namespace Business.Interfaces;

public interface IUserService
{
    Task<ServiceResult<object>> GetUsersAsync();
    Task<ServiceResult<object>> AddUserToRole(string userId, string roleName);
    Task<ServiceResult<object>> CreateUserAsync(SignUpFormData formData, string roleName);
    Task<ServiceResult<object>> GetUserByIdAsync(string userId);
}
