using HRApplicantSystem.Data.Models;

namespace HRApplicantSystem.Data.Repositories;

public interface IApplicationStatusHistoryRepository
{
    Task<IEnumerable<ApplicationStatusHistory>> GetByApplicationIdAsync(string applicationId);
    Task<bool> CreateAsync(ApplicationStatusHistory history);
}
