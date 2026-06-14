using HRApplicantSystem.Data.Models;

namespace HRApplicantSystem.Data.Repositories;

public interface IApplicantRepository
{
    Task<IEnumerable<Applicant>> GetAllAsync();
    Task<Applicant?> GetByIdAsync(string id);
    Task<ApplicantAccount?> GetByAccountIdAsync(string accountId);
    Task<bool> CreateAsync(ApplicantAccount account);
    Task<bool> UpdateAsync(ApplicantAccount account);
    Task<bool> DeleteAsync(string id);
}
