using HRApplicantSystem.Data.Models;

namespace HRApplicantSystem.Services.Interfaces;

public interface IAuditTrailService
{

    Task<bool> LogAsync(string? userId, string action, string tableAffected, string recordId);
    Task<IEnumerable<AuditTrail>> GetAllAsync();
    Task<IEnumerable<AuditTrail>> GetByUserIdAsync(string userId);
}