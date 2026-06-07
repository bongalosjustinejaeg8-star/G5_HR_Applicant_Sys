using HRApplicantSystem.Data.Models;

namespace HRApplicantSystem.Services.Interfaces;

public interface IAuthService
{
    Task<ApplicantAccount?> LoginApplicantAsync(string email, string password);
    Task<User?> LoginHRAsync(string email, string password);
    Task<bool> RegisterApplicantAsync(string email, string password);
    Task<bool> ChangePasswordAsync(string accountId, string newPassword);

}