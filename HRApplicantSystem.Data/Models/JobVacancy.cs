using HRApplicantSystem.Shared.Enums;

namespace HRApplicantSystem.Data.Models;

public class JobVacancy
{
    public string VacancyId { get; set; } = string.Empty;
    public string DepartmentId { get; set; } = string.Empty;
    public string PositionTitle { get; set; } = string.Empty;
    public string Qualifications { get; set; } = string.Empty;
    public string EmploymentType { get; set; } = string.Empty;
    public VacancyStatus Status { get; set; } = VacancyStatus.Open;
    public DateTime CreatedAt { get; set; }
}