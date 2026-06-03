using HRApplicantSystem.Shared.Enums;

namespace HRApplicantSystem.Data.Models;

public class Application
{
    public string ApplicationId { get; set; } = string.Empty;
    public string ApplicantId { get; set; } = string.Empty;
    public string VacancyId { get; set; } = string.Empty;
    public ApplicationStatus Status { get; set; } = ApplicationStatus.Draft;
    public DateTime SubmittedAt { get; set; } 
    public bool IsLocked { get; set; } = false;

}