using HRApplicantSystem.Data.Models;

namespace HRApplicantSystem.Data.Repositories;

public interface IAuditTrailRepository
{
    Task<IEnumerable<AuditTrail>> GetAllAsync();
    Task<IEnumerable<AuditTrail>> GetByUserIdAsync(string userId);
    Task<bool> CreateAsync(AuditTrail audit);
}
