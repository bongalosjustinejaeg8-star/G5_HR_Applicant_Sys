using HRApplicantSystem.Shared.Enums;

namespace HRApplicantSystem.Data.Models;

public class ApplicantDocument
{
    public string DocumentId { get; set; } = string.Empty;
    public string ApplicantId { get; set; } = string.Empty;
    public string RequirementTypeId { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public DocumentStatus Status { get; set; } = DocumentStatus.Missing;
    public string Remarks { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }


}