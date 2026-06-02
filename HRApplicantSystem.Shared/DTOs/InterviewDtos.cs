using HRApplicantSystem.Shared.Enums;

namespace HRApplicantSystem.Shared.DTOs;

public class InterviewDto
{
    public InterviewMode InterviewMode { get; set; }
    public DateTime InterviewDate { get; set; }
    public string? InterviewId { get; set; }

    public string? ApplicantName { get; set; }
    public string? JobTitle { get; set; }

    public InterviewStatus InterviewStatus { get; set; }

    public string? Location { get; set; }

}