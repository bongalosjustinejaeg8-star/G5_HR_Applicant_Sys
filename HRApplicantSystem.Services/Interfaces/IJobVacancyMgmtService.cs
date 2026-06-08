using HRApplicantSystem.Data.Models;

namespace HRApplicantSystem.Services.Interfaces;

public interface IJobVacancyMgmtService
{
    Task<IEnumerable<JobVacancy>> GetAllJobsAsync();
    Task<JobVacancy?> GetJobByIdAsync(string vacancyId);
    Task<bool> CreateJobVacancyAsync(JobVacancy vacancy);
    Task<bool> UpdateJobVacancyAsync(JobVacancy vacancy);
    Task<bool> CloseJobVacancyAsync(string vacancyId);
    Task<bool> OpenJobVacancyAsync(string vacancyId);
}
