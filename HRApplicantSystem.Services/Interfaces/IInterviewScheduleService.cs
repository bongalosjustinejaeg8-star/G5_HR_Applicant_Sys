using HRApplicantSystem.Data.Models;
using HRApplicantSystem.Shared.Enums;

namespace HRApplicantSystem.Services.Interfaces;

public interface IInterviewScheduleService
{
    Task<IEnumerable<Application>> GetShortlistedApplicationsAsync();

    Task<string> ScheduleInterviewAsync(
        string applicationId,
        string hrUserId,
        DateTime interviewDate,
        string interviewer,
        InterviewMode mode,
        string location);
}