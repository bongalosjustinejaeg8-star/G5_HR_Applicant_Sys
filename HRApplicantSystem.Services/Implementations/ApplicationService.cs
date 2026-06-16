using HRApplicantSystem.Data.Models;
using HRApplicantSystem.Data.Repositories;
using HRApplicantSystem.Shared.Enums;
using HRApplicantSystem.Services.Interfaces;

namespace HRApplicantSystem.Services.Implementations;

public class ApplicationService : IApplicationService
{
    private readonly IApplicationRepository _applicationRepo;
    private readonly IApplicationStatusHistoryRepository _historyRepo;
    private readonly IJobVacancyRepository _jobVacancyRepo;

    public ApplicationService(
        IApplicationRepository applicationRepo,
        IApplicationStatusHistoryRepository historyRepo,
        IJobVacancyRepository jobVacancyRepo)
    {
        _applicationRepo = applicationRepo;
        _historyRepo = historyRepo;
        _jobVacancyRepo = jobVacancyRepo;
    }

    public async Task<bool> SubmitApplicationAsync(string applicantId, string vacancyId)
    {
        var vacancy = await _jobVacancyRepo.GetByIdAsync(vacancyId);
        if (vacancy == null) return false;

        // step 2: block if vacancy is closed
        if (vacancy.Status == VacancyStatus.Closed) return false;

        // step 3: check for duplicate application
        bool alreadyApplied = await _applicationRepo.ExistsAsync(applicantId, vacancyId);
        if (alreadyApplied) return false;

        // step 4: build the application object
        var application = new Application
        {
            ApplicantId = applicantId,
            VacancyId = vacancyId,
            Status = ApplicationStatus.Draft,
            IsLocked = false
        };

        // step 5: save to database
        bool created = await _applicationRepo.CreateAsync(application);
        if (!created) return false;

        // step 6: immediately move status to Submitted
        await _applicationRepo.UpdateStatusAsync(application.ApplicationId, ApplicationStatus.Submitted);
        application.Status = ApplicationStatus.Submitted;

        // step 7: log the status change
        await LogStatusChangeAsync(application.ApplicationId, null, ApplicationStatus.Submitted, null, "Application submitted");

        return true;
    }

    public async Task<bool> CanEditAsync(string applicationId)
    {
        // locked applications cannot be edited — IsLocked is set when HR starts review
        var application = await _applicationRepo.GetByIdAsync(applicationId);
        if (application == null) return false;
        return !application.IsLocked;
    }

    public async Task<Application?> GetByIdAsync(string applicationId)
    {
        // pass through to repository
        return await _applicationRepo.GetByIdAsync(applicationId);
    }

    public async Task<IEnumerable<Application>> GetByApplicantIdAsync(string applicantId)
    {
        // pass through to repository
        return await _applicationRepo.GetByApplicantIdAsync(applicantId);
    }

    public async Task<IEnumerable<Application>> GetAllAsync()
    {
        // pass through to repository
        return await _applicationRepo.GetAllAsync();
    }

    public async Task<bool> StartReviewAsync(string applicationId, string changedBy)
    {
        // step 1: get the application
        var application = await _applicationRepo.GetByIdAsync(applicationId);
        if (application == null) return false;

        // step 2: lock so applicant can no longer edit
        await _applicationRepo.LockAsync(applicationId);

        // step 3: move status to UnderReview
        var oldStatus = application.Status;
        await _applicationRepo.UpdateStatusAsync(applicationId, ApplicationStatus.UnderReview);

        // step 4: log the status change — changedBy is an HR user_id here
        await LogStatusChangeAsync(applicationId, oldStatus, ApplicationStatus.UnderReview, changedBy, "HR started review");

        return true;
    }

    public async Task<bool> ChangeStatusAsync(string applicationId, ApplicationStatus newStatus, string changedBy, string? remarks)
    {
        // step 1: get current application
        var application = await _applicationRepo.GetByIdAsync(applicationId);
        if (application == null) return false;

        // step 2: capture old status for history
        var oldStatus = application.Status;

        // step 3: apply the new status
        await _applicationRepo.UpdateStatusAsync(applicationId, newStatus);

        // step 4: log every status change — powers the applicant's status timeline
        await LogStatusChangeAsync(applicationId, oldStatus, newStatus, changedBy, remarks);

        return true;
    }

    // internal helper — writes a row to ApplicationStatusHistory
    // changedBy must be a valid user_id from Users table, or null for applicant-initiated actions
    private async Task LogStatusChangeAsync(
        string applicationId,
        ApplicationStatus? oldStatus,
        ApplicationStatus newStatus,
        string? changedBy,
        string? remarks)
    {
        var history = new ApplicationStatusHistory
        {
            ApplicationId = applicationId,
            OldStatus = oldStatus,
            NewStatus = newStatus,
            ChangedBy = changedBy,
            ChangedAt = DateTime.Now,
            Remarks = remarks
        };

        await _historyRepo.CreateAsync(history);
    }
}