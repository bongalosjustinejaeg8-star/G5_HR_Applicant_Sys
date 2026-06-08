using HRApplicantSystem.Data.Models;
using HRApplicantSystem.Data.Repositories;
using HRApplicantSystem.Services.Interfaces;
using HRApplicantSystem.Shared.Enums;

namespace HRApplicantSystem.Services.Implementations;

public class ScreeningService : IScreeningService
{
    private readonly IApplicationRepository _applicationRepository;
    private readonly IScreeningResultRepository _screeningRepository;
    private readonly IApplicationStatusHistoryRepository _historyRepository;

    public ScreeningService(IApplicationRepository applicationRepository,
                            IScreeningResultRepository screeningRepository,
                            IApplicationStatusHistoryRepository historyRepository)
    {
        _applicationRepository = applicationRepository;
        _screeningRepository = screeningRepository;
        _historyRepository = historyRepository;
    }

    public async Task<IEnumerable<Application>> GetApplicationsForScreeningAsync()
    {
        var apps = await _applicationRepository.GetAllAsync();
        return apps.Where(a => a.Status == ApplicationStatus.UnderReview);
    }

    // Based on PDF: Qualified = Shortlisted, NotQualified = Rejected
    public async Task<string> SaveScreeningResultAsync(string applicationId, string hrUserId, ScreeningResults result, string remarks)
    {
        var newStatus = result == ScreeningResults.Qualified
            ? ApplicationStatus.Shortlisted
            : ApplicationStatus.Rejected;

        var screeningResult = new ScreeningResult
        {
            ApplicationId = applicationId,
            ScreenedBy = hrUserId,
            Result = result.ToString(),
            Remarks = remarks
        };

        await _screeningRepository.CreateAsync(screeningResult);

        bool success = await _applicationRepository.UpdateStatusAsync(applicationId, newStatus);

        if (success)
        {
            await _historyRepository.CreateAsync(new ApplicationStatusHistory
            {
                ApplicationId = applicationId,
                ChangedBy = hrUserId,
                OldStatus = "UnderReview",
                NewStatus = newStatus.ToString()
            });
            return $"Applicant marked as {newStatus}.";
        }

        return "Something went wrong.";
    }
}