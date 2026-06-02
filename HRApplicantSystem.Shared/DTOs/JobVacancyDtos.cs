using HRApplicantSystem.Shared.Enums;

namespace HRApplicantSystem.Shared.DTOs;

public class JobVacancyDto
{
    public VacancyStatus VacancyStatus { get; set; }
    public string? PositionTitle { get; set; }
    public string? Department { get; set; }
    public string? JobDescription { get; set; }
    public string? VacancyId { get; set; }
}