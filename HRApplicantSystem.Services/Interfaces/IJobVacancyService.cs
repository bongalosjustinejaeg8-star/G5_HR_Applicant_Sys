using HRApplicantSystem.Data.Models;

namespace HRApplicantSystem.Services.Interfaces;

public interface IJobVacancyService
{
    Task<IEnumerable<JobVacancy>> GetOpenJobsAsync();
    Task<IEnumerable<JobVacancy>> SearchJobsAsync(string keyword);
    Task<JobVacancy?> GetJobByIdAsync(string vacancyId);
}