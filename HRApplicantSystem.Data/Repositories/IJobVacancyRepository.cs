using HRApplicantSystem.Data.Models;
using HRApplicantSystem.Shared.Enums;

namespace HRApplicantSystem.Data.Repositories;

public interface IJobVacancyRepository
{
    Task<IEnumerable<JobVacancy>> GetAllAsync();
    Task<IEnumerable<JobVacancy>> GetOpenAsync();
    Task<JobVacancy?> GetByIdAsync(string id);
    Task<bool> CreateAsync(JobVacancy vacancy);
    Task<bool> UpdateAsync(JobVacancy vacancy);
    Task<bool> UpdateStatusAsync(string id, VacancyStatus status);
    Task<bool> DeleteAsync(string id);
}
