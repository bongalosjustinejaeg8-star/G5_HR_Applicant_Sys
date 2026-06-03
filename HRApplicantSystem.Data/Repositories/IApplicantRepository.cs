using HRApplicantSystem.Data.Models;

namespace HRApplicantSystem.Data.Repositories;

public interface IApplicantRepository
{
    Task<IEnumerable<Applicant>> GetAllAsync();
    Task<Applicant?> GetByIdAsync(string id);
    Task<Applicant?> GetByAccountIdAsync(string accountId);
    Task<bool> CreateAsync(Applicant applicant);
    Task<bool> UpdateAsync(Applicant applicant);
    Task<bool> DeleteAsync(string id);
}
