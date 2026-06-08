using HRApplicantSystem.Data.Models;
using HRApplicantSystem.Data.Repositories;
using HRApplicantSystem.Services.Interfaces;
using HRApplicantSystem.Shared.Enums;

namespace HRApplicantSystem.Services.Implementations;

public class InterviewScheduleService : IInterviewScheduleService
{
    private readonly IApplicationRepository _applicationRepository;
    private readonly IInterviewScheduleRepository _scheduleRepository;
    private readonly IApplicationStatusHistoryRepository _historyRepository;

    public InterviewScheduleService(IApplicationRepository applicationRepository,
                                    IInterviewScheduleRepository scheduleRepository,
                                    IApplicationStatusHistoryRepository historyRepository)
    {
        _applicationRepository = applicationRepository;
        _scheduleRepository = scheduleRepository;
        _historyRepository = historyRepository;
    }

    public async Task<IEnumerable<Application>> GetShortlistedApplicationsAsync()
    {
        var apps = await _applicationRepository.GetAllAsync();
        return apps.Where(a => a.Status == ApplicationStatus.Shortlisted);
    }

    // Based on PDF: block past dates, update status to ForInterview
    public async Task<string> ScheduleInterviewAsync(string applicationId, string hrUserId, DateTime interviewDate, string interviewer, InterviewMode mode, string location)
    {
        if (interviewDate < DateTime.Now)
            return "Interview date cannot be in the past.";

        if (string.IsNullOrWhiteSpace(interviewer))
            return "Please enter an interviewer name.";

        var schedule = new InterviewSchedule
        {
            ApplicationId = applicationId,
            InterviewDate = interviewDate,
            Mode = InterviewMode.Online,
            Location = location,
            Status = InterviewStatus.Scheduled
        };

        await _scheduleRepository.CreateAsync(schedule);

        bool success = await _applicationRepository.UpdateStatusAsync(
            applicationId, ApplicationStatus.ForInterview);

        if (success)
        {
            await _historyRepository.CreateAsync(new ApplicationStatusHistory
            {
                ApplicationId = applicationId,
                ChangedBy = hrUserId,
                OldStatus = ApplicationStatus.UnderReview,
                NewStatus = ApplicationStatus.ForInterview
            });
            return "Interview scheduled successfully!";
        }

        return "Something went wrong.";
    }
}