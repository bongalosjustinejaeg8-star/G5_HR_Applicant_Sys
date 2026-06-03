namespace HRApplicantSystem.Data.Models;

public class InterviewEvaluation
{
    public string Eval_Id { get; set; } = string.Empty;
    public string ScheduleId { get; set; } = string.Empty;
    public string EvaluatedBy { get; set; } = string.Empty;
    public int? Score { get; set; }

    public string Remarks { get; set; } = string.Empty;
    public string? Recommendation { get; set; }

    public string PassFail { get; set; } = string.Empty;
}