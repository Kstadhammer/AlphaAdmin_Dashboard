using Business.Models;
using Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Business.Interfaces;

public interface IAuthService
{
    Task<SignInResult> SignInAsync(SignInFormData signInFormData);
    Task<AuthResult> SignUpAsync(SignUpFormData signUpFormData);
    Task<bool> LoginAsync(MemberLoginForm form);
    Task<bool> SignUpAsync(MemberSignUpForm form);
    Task LogoutAsync();
}
