using HRApplicantSystem.Data.Models;
using HRApplicantSystem.Data.Repositories;
using HRApplicantSystem.Services.Interfaces;
using HRApplicantSystem.Shared.Enums;

namespace HRApplicantSystem.Services.Implementations;

public class ApplicationService : IApplicationService
{
    private readonly IApplicationRepository _applicationRepository;
    private readonly IApplicationStatusHistoryRepository _historyRepository;

    public ApplicationService(IApplicationRepository applicationRepository,
                              IApplicationStatusHistoryRepository historyRepository)
    {
        _applicationRepository = applicationRepository;
        _historyRepository = historyRepository;
    }

    public async Task<IEnumerable<Application>> GetByApplicantIdAsync(string applicantId)
    {
        return await _applicationRepository.GetByApplicantIdAsync(applicantId);
    }

    public async Task<IEnumerable<Application>> GetAllSubmittedAsync()
    {
        var apps = await _applicationRepository.GetAllAsync();
        return apps.Where(a => a.Status == ApplicationStatus.Submitted);
    }

    // Based on PDF: block duplicate applications
    public async Task<string> ApplyAsync(string applicantId, string vacancyId)
    {
        bool alreadyApplied = await _applicationRepository.ExistsAsync(applicantId, vacancyId);
        if (alreadyApplied)
            return "You have already applied to this job.";

        var application = new Application
        {
            ApplicantId = applicantId,
            VacancyId = vacancyId,
            Status = ApplicationStatus.Draft
        };

        bool success = await _applicationRepository.CreateAsync(application);
        return success ? "Application created successfully!" : "Something went wrong.";
    }

    // Based on PDF: only Draft can be submitted
    public async Task<string> SubmitApplicationAsync(string applicationId, string applicantId)
    {
        var app = await _applicationRepository.GetByIdAsync(applicationId);
        if (app == null) return "Application not found.";

        if (app.Status != ApplicationStatus.Draft)
            return "Only Draft applications can be submitted.";

        bool success = await _applicationRepository.UpdateStatusAsync(
            applicationId, ApplicationStatus.Submitted);

        if (success)
        {
            await _historyRepository.CreateAsync(new ApplicationStatusHistory
            {
                ApplicationId = applicationId,
                OldStatus = "Draft",
                NewStatus = "Submitted"
            });
            return "Application submitted successfully!";
        }

        return "Something went wrong.";
    }

    // Based on PDF: locks application and changes status to Under Review
    public async Task<string> StartReviewAsync(string applicationId, string hrUserId)
    {
        var app = await _applicationRepository.GetByIdAsync(applicationId);
        if (app == null) return "Application not found.";

        if (app.Status != ApplicationStatus.Submitted)
            return "Only Submitted applications can be reviewed.";

        await _applicationRepository.LockAsync(applicationId);

        bool success = await _applicationRepository.UpdateStatusAsync(
            applicationId, ApplicationStatus.UnderReview);

        if (success)
        {
            await _historyRepository.CreateAsync(new ApplicationStatusHistory
            {
                ApplicationId = applicationId,
                ChangedBy = hrUserId,
                OldStatus = "Submitted",
                NewStatus = "UnderReview"
            });
            return "Review started. Application is now locked.";
        }

        return "Something went wrong.";
    }
}