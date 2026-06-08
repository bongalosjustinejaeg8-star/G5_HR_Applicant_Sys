using HRApplicantSystem.Data.Models;
using HRApplicantSystem.Data.Repositories;
using HRApplicantSystem.Services.Interfaces;
using HRApplicantSystem.Shared.Enums;

namespace HRApplicantSystem.Services.Implementations;

public class JobVacancyMgmtService : IJobVacancyMgmtService
{
    private readonly IJobVacancyRepository _jobVacancyRepository;

    public JobVacancyMgmtService(IJobVacancyRepository jobVacancyRepository)
    {
        _jobVacancyRepository = jobVacancyRepository;
    }

    public async Task<IEnumerable<JobVacancy>> GetAllJobsAsync()
    {
        return await _jobVacancyRepository.GetAllAsync();
    }

    public async Task<JobVacancy?> GetJobByIdAsync(string vacancyId)
    {
        return await _jobVacancyRepository.GetByIdAsync(vacancyId);
    }

    public async Task<bool> CreateJobVacancyAsync(JobVacancy vacancy)
    {
        return await _jobVacancyRepository.CreateAsync(vacancy);
    }

    public async Task<bool> UpdateJobVacancyAsync(JobVacancy vacancy)
    {
        return await _jobVacancyRepository.UpdateAsync(vacancy);
    }

    public async Task<bool> CloseJobVacancyAsync(string vacancyId)
    {
        return await _jobVacancyRepository.UpdateStatusAsync(vacancyId, VacancyStatus.Closed);
    }

    public async Task<bool> OpenJobVacancyAsync(string vacancyId)
    {
        return await _jobVacancyRepository.UpdateStatusAsync(vacancyId, VacancyStatus.Open);
    }
}