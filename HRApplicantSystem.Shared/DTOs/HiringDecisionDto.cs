using System.Dynamic;

namespace HRApplicantSystem.Shared.DTOs;

public class HiringDecisionDto
{
    public string? DecisionID{ get; set; }
    public string? ApplicationID { get; set; }
    public string? DecidedBy { get; set; }
    public string? ApplicantName{ get; set; }
    public string? PositionTitle{ get; set; }
    public string? Decision { get; set; }
    public DateTime DecidedAt{ get; set; }
    public string? Remarks{ get; set; }

}