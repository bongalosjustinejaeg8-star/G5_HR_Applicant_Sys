namespace HRApplicantSystem.Data.Models;

public class Position
{
    public string PositionId { get; set; } = string.Empty;
    public string DepartmentId { get; set; } = string.Empty;
    public string PositionTitle { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
