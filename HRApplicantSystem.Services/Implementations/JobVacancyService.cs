using HRApplicantSystem.Data.Models;
using HRApplicantSystem.Data.Repositories;
using HRApplicantSystem.Services.Interfaces;
using HRApplicantSystem.Shared.Enums;

namespace HRApplicantSystem.Services.Implementations;

public class JobVacancyService : IJobVacancyService
{
    private readonly IJobVacancyRepository _jobVacancyRepository;

    public JobVacancyService(IJobVacancyRepository jobVacancyRepository)
    {
        _jobVacancyRepository = jobVacancyRepository;
    }

    public async Task<IEnumerable<JobVacancy>> GetOpenJobsAsync()
    {
        return await _jobVacancyRepository.GetOpenAsync();
    }

    public async Task<IEnumerable<JobVacancy>> SearchJobsAsync(string keyword)
    {
        var jobs = await _jobVacancyRepository.GetOpenAsync();
        return jobs.Where(j =>
            j.PositionTitle.Contains(keyword, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<JobVacancy?> GetJobByIdAsync(string vacancyId)
    {
        return await _jobVacancyRepository.GetByIdAsync(vacancyId);
    }
}