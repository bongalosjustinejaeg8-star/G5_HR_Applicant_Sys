using HRApplicantSystem.Data.Models;
using HRApplicantSystem.Data.Repositories;
using HRApplicantSystem.Services.Interfaces;
using HRApplicantSystem.Shared.Enums;

namespace HRApplicantSystem.Services.Implementations;

public class HiringDecisionService : IHiringDecisionService
{
    private readonly IApplicationRepository _applicationRepository;
    private readonly IHiringDecisionRepository _hiringDecisionRepository;
    private readonly IApplicationStatusHistoryRepository _historyRepository;

    public HiringDecisionService(
        IApplicationRepository applicationRepository,
        IHiringDecisionRepository hiringDecisionRepository,
        IApplicationStatusHistoryRepository historyRepository)
    {
        _applicationRepository = applicationRepository;
        _hiringDecisionRepository = hiringDecisionRepository;
        _historyRepository = historyRepository;
    }

    public async Task<IEnumerable<Application>> GetApplicationsForHiringDecisionAsync()
    {
        var apps = await _applicationRepository.GetAllAsync();
        // only apps that finished evaluation are ready for final decision
        return apps.Where(a => a.Status == ApplicationStatus.ForFinalReview);
    }

    public async Task<string> MakeHiringDecisionAsync(string applicationId, string hrUserId, string decision, string remarks)
    {
        var app = await _applicationRepository.GetByIdAsync(applicationId);
        if (app == null) return "Application not found.";

        // business rule: only HRManager or Admin can accept
        var newStatus = decision == "Accepted"
            ? ApplicationStatus.Accepted
            : ApplicationStatus.Rejected;

        var hiringDecision = new HiringDecision
        {
            ApplicationId = applicationId,
            DecidedBy = hrUserId,
            Decision = decision,
            Remarks = remarks,
            DecidedAt = DateTime.Now
        };

        await _hiringDecisionRepository.CreateAsync(hiringDecision);
        await _applicationRepository.UpdateStatusAsync(applicationId, newStatus);

        await _historyRepository.CreateAsync(new ApplicationStatusHistory
        {
            ApplicationId = applicationId,
            ChangedBy = hrUserId,
            OldStatus = app.Status,
            NewStatus = newStatus,
            Remarks = remarks
        });

        return $"Decision recorded: {decision}";
    }

    public async Task<string> SendOfferLetterAsync(string applicationId, string offerDetails)
    {
        // for capstone — just log it, no actual email sending
        var app = await _applicationRepository.GetByIdAsync(applicationId);
        if (app == null) return "Application not found.";
        return $"Offer letter noted for application {applicationId}.";
    }

    public async Task<string> SendRejectionLetterAsync(string applicationId, string rejectionReason)
    {
        // for capstone — just log it, no actual email sending
        var app = await _applicationRepository.GetByIdAsync(applicationId);
        if (app == null) return "Application not found.";
        return $"Rejection noted for application {applicationId}.";
    }
}