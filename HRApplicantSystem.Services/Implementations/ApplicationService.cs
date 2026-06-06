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
        // step 1: check if vacancy exists and is still open
        var vacancy = await _jobVacancyRepo.GetByIdAsync(vacancyId);
        if (vacancy == null) return false;

        // step 2: block if vacancy is closed
        // business rule: closed jobs cant receive applications
        if (vacancy.Status == VacancyStatus.Closed) return false;

        // step 3: check for duplicate application
        // business rule: same applicant cant apply twice to same job
        bool alreadyApplied = await _applicationRepo.ExistsAsync(applicantId, vacancyId);
        if (alreadyApplied) return false;

        // step 4: create the application
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

        // step 6: log the initial status in history
        await LogStatusChangeAsync(application.ApplicationId, null, ApplicationStatus.Draft, applicantId, "Application created");

        return true;
    }

    public async Task<bool> CanEditAsync(string applicationId)
    {
        // get the application
        var application = await _applicationRepo.GetByIdAsync(applicationId);
        if (application == null) return false;

        // business rule: locked applications cannot be edited
        // IsLocked becomes true when HR starts review
        return !application.IsLocked;
    }

    public async Task<Application?> GetByIdAsync(string applicationId)
    {
        // just pass through to repository
        return await _applicationRepo.GetByIdAsync(applicationId);
    }

    public async Task<IEnumerable<Application>> GetByApplicantIdAsync(string applicantId)
    {
        // just pass through to repository
        return await _applicationRepo.GetByApplicantIdAsync(applicantId);
    }

    public async Task<IEnumerable<Application>> GetAllAsync()
    {
        // just pass through to repository
        return await _applicationRepo.GetAllAsync();
    }

    public async Task<bool> StartReviewAsync(string applicationId, string changedBy)
    {
        // step 1: get the application
        var application = await _applicationRepo.GetByIdAsync(applicationId);
        if (application == null) return false;

        // step 2: lock the application
        // business rule: once HR starts review, applicant cannot edit
        await _applicationRepo.LockAsync(applicationId);

        // step 3: change status to UnderReview
        var oldStatus = application.Status;
        await _applicationRepo.UpdateStatusAsync(applicationId, ApplicationStatus.UnderReview);

        // step 4: log the status change in history
        // business rule: every status change must be recorded
        await LogStatusChangeAsync(applicationId, oldStatus, ApplicationStatus.UnderReview, changedBy, "HR started review");

        return true;
    }

    public async Task<bool> ChangeStatusAsync(string applicationId, ApplicationStatus newStatus, string changedBy, string? remarks)
    {
        // step 1: get current application
        var application = await _applicationRepo.GetByIdAsync(applicationId);
        if (application == null) return false;

        // step 2: save old status for history
        var oldStatus = application.Status;

        // step 3: update the status
        await _applicationRepo.UpdateStatusAsync(applicationId, newStatus);

        // step 4: always log every status change
        // this is what powers the applicant's status timeline
        await LogStatusChangeAsync(applicationId, oldStatus, newStatus, changedBy, remarks);

        return true;
    }

    // private helper — used internally to write status history
    // not exposed in the interface since UI never calls this directly
    private async Task LogStatusChangeAsync(
        string applicationId,
        ApplicationStatus? oldStatus,
        ApplicationStatus newStatus,
        string changedBy,
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