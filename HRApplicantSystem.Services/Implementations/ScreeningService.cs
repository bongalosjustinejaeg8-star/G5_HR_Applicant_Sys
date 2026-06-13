using HRApplicantSystem.Data.Models;
using HRApplicantSystem.Data.Repositories;
using HRApplicantSystem.Services.Interfaces;
using HRApplicantSystem.Shared.Enums;

namespace HRApplicantSystem.Services.Implementations;

public class ScreeningService : IScreeningService
{
    private readonly IApplicationRepository _applicationRepository;
    private readonly IApplicationStatusHistoryRepository _historyRepository;
    private readonly IScreeningResultRepository _screeningRepository; 

    public ScreeningService(IApplicationRepository applicationRepository,
                       IApplicationStatusHistoryRepository historyRepository,
                       IScreeningResultRepository screeningRepository) 
    {
        _applicationRepository = applicationRepository;
        _historyRepository = historyRepository;
        _screeningRepository = screeningRepository; 
    }

    public async Task<IEnumerable<Application>> GetApplicationsForScreeningAsync()
    {
        var apps = await _applicationRepository.GetAllAsync();
        return apps.Where(a => a.Status == ApplicationStatus.UnderReview);
    }

    public async Task<string> SaveScreeningResultAsync(string applicationId, string hrUserId, ScreeningResults result, string remarks)
    {
        var app = await _applicationRepository.GetByIdAsync(applicationId);
        if (app == null) return "Application not found.";

        var newStatus = result == ScreeningResults.Qualified 
            ? ApplicationStatus.Shortlisted 
            : ApplicationStatus.Rejected;

        var screeningResult = new ScreeningResult
        {
            ApplicationId = applicationId,
            ScreenedBy = hrUserId,
            Result = result,
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
                OldStatus = ApplicationStatus.UnderReview,
                NewStatus = newStatus
            });
            return $"Screening result saved successfully! Status: {newStatus}";
        }

        return "Something went wrong.";
    }
}
