using HRApplicantSystem.Data.Models;
using HRApplicantSystem.Data.Repositories;
using HRApplicantSystem.Services.Interfaces;

namespace HRApplicantSystem.Services.Implementations;

public class AuditTrailService : IAuditTrailService
{
    private readonly IAuditTrailRepository _auditTrailRepo;

    public AuditTrailService(IAuditTrailRepository auditTrailRepo)
    {
        _auditTrailRepo = auditTrailRepo;
    }


    public async Task<bool> LogAsync(string? userId, string action, string tableAffected, string recordId)
    {
        var audit = new AuditTrail
        {
            UserId = userId,
            Action = action,
            TableAffected = tableAffected,
            RecordId = recordId,
            PerformedAt = DateTime.Now
        };

        return await _auditTrailRepo.CreateAsync(audit);
    }
    public async Task<IEnumerable<AuditTrail>> GetAllAsync()
    {
        return await _auditTrailRepo.GetAllAsync();
    }

    public async Task<IEnumerable<AuditTrail>> GetByUserIdAsync(string userId)
    {
        return await _auditTrailRepo.GetByUserIdAsync(userId);
    }



}

