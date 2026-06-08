using HRApplicantSystem.Data.Models;
using HRApplicantSystem.Data.Repositories;
using HRApplicantSystem.Services.Interfaces;
using HRApplicantSystem.Shared.Enums;

namespace HRApplicantSystem.Services.Implementations;

public class JobVacanciesService : IJobVacanciesService
{
    private readonly IJobVacancyRepository _jobVacancyRepository;

    public JobVacanciesService(IJobVacancyRepository jobVacancyRepository)
    {
        _jobVacancyRepository = jobVacancyRepository;
    }

    public async Task<IEnumerable<JobVacancy>> GetAllOpenJobsAsync()
    {
        return await _jobVacancyRepository.GetOpenAsync();
    }

    public async Task<IEnumerable<JobVacancy>> SearchJobsAsync(string keyword)
    {
        var all = await _jobVacancyRepository.GetOpenAsync();
        if (string.IsNullOrWhiteSpace(keyword)) return all;

        return all.Where(j =>
            j.PositionTitle.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
            (j.Qualifications != null && j.Qualifications.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
            j.EmploymentType.Contains(keyword, StringComparison.OrdinalIgnoreCase)
        );
    }

    public async Task<JobVacancy?> GetJobByIdAsync(string vacancyId)
    {
        return await _jobVacancyRepository.GetByIdAsync(vacancyId);
    }

    public async Task<IEnumerable<JobVacancy>> GetJobsByCategoryAsync(string category)
    {
        var all = await _jobVacancyRepository.GetOpenAsync();
        if (string.IsNullOrWhiteSpace(category)) return all;

        return all.Where(j =>
            j.DepartmentId.Contains(category, StringComparison.OrdinalIgnoreCase)
        );
    }
}