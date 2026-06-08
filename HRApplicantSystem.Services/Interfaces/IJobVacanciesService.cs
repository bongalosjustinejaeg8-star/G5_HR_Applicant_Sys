using HRApplicantSystem.Data.Models;

namespace HRApplicantSystem.Services.Interfaces;

public interface IJobVacanciesService
{
    Task<IEnumerable<JobVacancy>> GetAllOpenJobsAsync();
    Task<IEnumerable<JobVacancy>> SearchJobsAsync(string keyword);
    Task<JobVacancy?> GetJobByIdAsync(string vacancyId);
    Task<IEnumerable<JobVacancy>> GetJobsByCategoryAsync(string category);
}
