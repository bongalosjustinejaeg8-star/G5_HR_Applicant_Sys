using HRApplicantSystem.Shared.Enums;

namespace  HRApplicantSystem.Shared.DTOs;


public class StatusHistoryDto
{
    public ApplicationStatus StatusBefore { get; set; }
    public string? Remarks { get; set; }
    public ApplicationStatus StatusNow { get; set; }
    public DateTime DateChanged{ get; set; }


}