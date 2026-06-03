using HRApplicantSystem.Shared.Enums;

namespace HRApplicantSystem.Data.Models;

public class ScreeningResult
{
    public string ScreeningId { get; set; } = string.Empty;
    public string ApplicationId { get; set; } = string.Empty;
    public string ScreenedBy { get; set; } = string.Empty;
    public ScreeningResults Result { get; set; } = ScreeningResults.NotQualified;
    public string Remarks { get; set; } = string.Empty;
    public DateTime ScreenedAt { get; set; } 
    
}