using HRApplicantSystem.Data.Models;
using HRApplicantSystem.Data.Repositories;
using HRApplicantSystem.Services.Interfaces;
using HRApplicantSystem.Shared.Enums;

namespace HRApplicantSystem.Services.Implementations;

public class InterviewEvaluationService : IInterviewEvaluationService
{
    private readonly IApplicationRepository _applicationRepository;
    private readonly IInterviewScheduleRepository _scheduleRepository;
    private readonly IInterviewEvaluationRepository _evaluationRepository;
    private readonly IApplicationStatusHistoryRepository _historyRepository;

    public InterviewEvaluationService(
        IApplicationRepository applicationRepository,
        IInterviewScheduleRepository scheduleRepository,
        IInterviewEvaluationRepository evaluationRepository,
        IApplicationStatusHistoryRepository historyRepository)
    {
        _applicationRepository = applicationRepository;
        _scheduleRepository = scheduleRepository;
        _evaluationRepository = evaluationRepository;
        _historyRepository = historyRepository;
    }

    public async Task<IEnumerable<Application>> GetApplicationsForEvaluationAsync()
    {
        var apps = await _applicationRepository.GetAllAsync();
        return apps.Where(a => a.Status == ApplicationStatus.ForInterview);
    }

    // Based on PDF: Pass = ForFinalReview, Fail = Rejected
    public async Task<string> SaveEvaluationAsync(string applicationId, string hrUserId, int score, string remarks, string passFail, string recommendation)
    {
        if (score < 1 || score > 100)
            return "Score must be between 1 and 100.";

        var schedule = await _scheduleRepository.GetByApplicationIdAsync(applicationId);
        if (schedule == null)
            return "No interview schedule found for this applicant.";

        var evaluation = new InterviewEvaluation
        {
            ScheduleId = schedule.ScheduleId,
            EvaluatedBy = hrUserId,
            Score = score,
            Remarks = remarks,
            Recommendation = recommendation,
            PassFail = passFail
        };

        await _evaluationRepository.CreateAsync(evaluation);

        var newStatus = passFail == "Pass"
            ? ApplicationStatus.ForFinalReview
            : ApplicationStatus.Rejected;

        bool success = await _applicationRepository.UpdateStatusAsync(applicationId, newStatus);

        if (success)
        {
            await _historyRepository.CreateAsync(new ApplicationStatusHistory
            {
                ApplicationId = applicationId,
                ChangedBy = hrUserId,
                OldStatus = ApplicationStatus.ForInterview,
                NewStatus = newStatus
            });
            return $"Evaluation saved. Applicant is now {newStatus}.";
        }

        return "Something went wrong.";
    }
}