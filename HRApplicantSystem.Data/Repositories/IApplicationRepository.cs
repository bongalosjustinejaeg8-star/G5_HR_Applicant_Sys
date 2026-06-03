using HRApplicantSystem.Data.Models;
using HRApplicantSystem.Shared.Enums;

namespace HRApplicantSystem.Data.Repositories;

public interface IApplicationRepository
{
    Task<IEnumerable<Application>> GetAllAsync();
    Task<Application?> GetByIdAsync(string id);
    Task<IEnumerable<Application>> GetByApplicantIdAsync(string applicantId);
    Task<bool> ExistsAsync(string applicantId, string vacancyId);
    Task<bool> CreateAsync(Application application);
    Task<bool> UpdateStatusAsync(string id, ApplicationStatus status);
    Task<bool> LockAsync(string id);
    Task<bool> DeleteAsync(string id);
}
