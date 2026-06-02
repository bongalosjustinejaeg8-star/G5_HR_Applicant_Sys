using HRApplicantSystem.Shared.Enums;

namespace HRApplicantSystem.Shared.DTOs;

public class ApplicantDto
{
    public string? ApplicantId { get; set; }
    public string? FullName { get; set; }
    public string? ContactNo { get; set; }
    public string? Email { get; set; }

    public ApplicationStatus Status { get; set; }


}