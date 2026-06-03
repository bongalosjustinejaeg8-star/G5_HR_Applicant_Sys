namespace HRApplicantSystem.Data.Models;

public class HiringDecision
{
    public string DecisionId { get; set; } = string.Empty;
    public string ApplicationId { get; set; } = string.Empty;
    public string DecidedBy { get; set; } = string.Empty;
    public string Decision { get; set; } = string.Empty;
    public string Remarks { get; set; } = string.Empty;
    public DateTime DecidedAt { get; set; }
}