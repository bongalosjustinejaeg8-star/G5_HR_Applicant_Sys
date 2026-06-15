using HRApplicantSystem.Data.Models;

namespace HRApplicantSystem.Data.Repositories;

public interface IApplicantAccountRepository
{
    Task<IEnumerable<ApplicantAccount>> GetAllAsync();
    Task<ApplicantAccount?> GetByIdAsync(string id);
    Task<ApplicantAccount?> GetByEmailAsync(string email);
    Task<bool> CreateAsync(ApplicantAccount account);
    Task<bool> UpdateAsync(ApplicantAccount account);
    Task<bool> DeleteAsync(string Id);
}
    