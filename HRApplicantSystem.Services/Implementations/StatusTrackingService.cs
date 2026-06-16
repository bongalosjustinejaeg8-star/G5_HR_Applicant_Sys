using HRApplicantSystem.Data.Models;
using HRApplicantSystem.Data.Repositories;
using HRApplicantSystem.Services.Interfaces;

namespace HRApplicantSystem.Services.Implementations;

public class StatusTrackingService : IStatusTrackingService
{
    private readonly IApplicationStatusHistoryRepository _historyRepo;
    private readonly IApplicationRepository _applicationRepo;

    public StatusTrackingService(
        IApplicationStatusHistoryRepository historyRepo,
        IApplicationRepository applicationRepo)
    {
        _historyRepo = historyRepo;
        _applicationRepo = applicationRepo;
    }

    // Returns all status history entries for a given application, ordered by date ascending.
    // Powers the applicant's status timeline view.
    public async Task<IEnumerable<ApplicationStatusHistory>> GetApplicationStatusHistoryAsync(string applicationId)
    {
        return await _historyRepo.GetByApplicationIdAsync(applicationId);
    }

    // Returns a summary object: latest status per application + counts by status,
    // for a given applicant. Used by the applicant dashboard.
    public async Task<dynamic?> GetApplicationStatusSummaryAsync(string applicantId)
    {
        var apps = await _applicationRepo.GetByApplicantIdAsync(applicantId);
        if (!apps.Any()) return null;

        var summary = apps
            .GroupBy(a => a.Status)
            .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
            .ToList();

        var latest = apps.OrderByDescending(a => a.SubmittedAt).First();

        return new
        {
            LatestApplicationId = latest.ApplicationId,
            LatestStatus = latest.Status.ToString(),
            StatusCounts = summary
        };
    }
}
