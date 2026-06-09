using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRApplicantSystem.Data;
using HRApplicantSystem.Data.Models;
using HRApplicantSystem.Data.Repositories;
using HRApplicantSystem.Services.Implementations;
using HRApplicantSystem.Services.Interfaces;
using HRApplicantSystem.Shared.Helpers;

namespace HRApplicantSystem.UI.ViewModels.Applicant;

public partial class JobVacanciesViewModel : ViewModelBase
{
    private readonly IJobVacancyService _jobVacancyService;
    private readonly IApplicationService _applicationService;

    [ObservableProperty] private ObservableCollection<JobVacancy> _vacancies = new();
    [ObservableProperty] private JobVacancy? _selectedJob;
    [ObservableProperty] private string _message = string.Empty;
    [ObservableProperty] private bool _hasMessage = false;

    public JobVacanciesViewModel(IJobVacancyService jobVacancyService, IApplicationService applicationService)
    {
        _jobVacancyService = jobVacancyService;
        _applicationService = applicationService;
        _ = LoadJobVacanciesAsync();
    }

    public async Task LoadJobVacanciesAsync()
    {
        try
        {
            var jobs = await _jobVacancyService.GetOpenJobsAsync();
            Vacancies.Clear();
            foreach (var job in jobs) Vacancies.Add(job);
        }
        catch (Exception ex) { Message = ex.Message; HasMessage = true; }
    }

    [RelayCommand]
    public async Task ApplyAsync()
    {
        if (SelectedJob == null) return;
        try
        {
            var db = new DbContext(AppConfig.ConnectionString);
            var applicantRepo = new ApplicantRepository(db);
            var applicant = await applicantRepo.GetByAccountIdAsync(SessionManager.CurrentUserId ?? string.Empty);
            if (applicant == null) { Message = "Please complete your profile first!"; HasMessage = true; return; }

            bool success = await _applicationService.SubmitApplicationAsync(applicant.ApplicantId, SelectedJob.VacancyId);
            Message = success ? "Application submitted successfully!" : "You may have already applied for this job.";
            HasMessage = true;
        }
        catch (Exception ex) { Message = ex.Message; HasMessage = true; }
    }
}