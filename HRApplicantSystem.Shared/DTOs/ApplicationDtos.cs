using HRApplicantSystem.Shared.Enums;

namespace HRApplicantSystem.Shared.DTOs;

public class ApplicationDto 
{
    public ApplicationStatus Status { get; set; }
    public string? ApplicationId{ get; set; }
    public string? ApplicantName { get; set; }
    public string? PositionTitle { get; set; }

    public DateTime SubmittedAt { get; set; }


}