using HRApplicantSystem.Data.Models;

namespace HRApplicantSystem.Services.Interfaces;

public interface IStatusTrackingService
{
    Task<IEnumerable<ApplicationStatusHistory>> GetApplicationStatusHistoryAsync(string applicationId);
    Task<dynamic?> GetApplicationStatusSummaryAsync(string applicantId);
}
