using HRApplicantSystem.Shared.Enums;

namespace HRApplicantSystem.Data.Models;

public class InterviewSchedule
{
    public string ScheduleId { get; set; } = string.Empty;
    public string ApplicationId { get; set; } = string.Empty;
    public string Interviewerid { get; set; } = string.Empty;
    public DateTime InterviewDate { get; set; }
    public InterviewMode Mode { get; set; } = InterviewMode.Onsite;
    public string Location { get; set; } = string.Empty;
    public InterviewStatus Status { get; set; } = InterviewStatus.Scheduled;

}