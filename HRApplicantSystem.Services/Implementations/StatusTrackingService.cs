using HRApplicantSystem.Data.Models;
using HRApplicantSystem.Services.Interfaces;

namespace HRApplicantSystem.Services.Implementations;

public class StatusTrackingService : IStatusTrackingService
{
    public async Task<IEnumerable<ApplicationStatusHistory>> GetApplicationStatusHistoryAsync(string applicationId)
    {
        // TODO: Implement retrieval of application status history
        return await Task.FromResult(Enumerable.Empty<ApplicationStatusHistory>());
    }

    public async Task<dynamic?> GetApplicationStatusSummaryAsync(string applicantId)
    {
        // TODO: Implement retrieval of status summary for applicant's applications
        // Should include count by status, latest status per application
        return await Task.FromResult<dynamic?>(null);
    }
}
