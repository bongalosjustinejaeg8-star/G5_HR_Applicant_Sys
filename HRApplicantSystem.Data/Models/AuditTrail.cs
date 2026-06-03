namespace HRApplicantSystem.Data.Models;

public class AuditTrail
{
    public string AuditId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string TableAffected { get; set; } = string.Empty;
    public string RecordId { get; set; } = string.Empty;
    public DateTime PerformedAt { get; set; }

}