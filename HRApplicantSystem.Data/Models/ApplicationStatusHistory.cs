using HRApplicantSystem.Shared.Enums;

namespace HRApplicantSystem.Data.Models;

public class ApplicationStatusHistory
{
    public string HistoryId { get; set; } = string.Empty;
    public string ApplicationId { get; set; } = string.Empty;
    public string? ChangedBy { get; set; } 
    public ApplicationStatus OldStatus { get; set; } 
    public ApplicationStatus NewStatus { get; set; } = ApplicationStatus.Draft;
    public DateTime ChangedAt { get; set; } 
    public string? Remarks{ get; set; }

}