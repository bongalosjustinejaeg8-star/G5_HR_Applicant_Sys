using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRApplicantSystem.Data.Models;
using HRApplicantSystem.Services.Interfaces;
using HRApplicantSystem.Shared.Helpers;
using HRApplicantSystem.Shared.Enums;
using System;
using HRApplicantSystem.Data;   
using HRApplicantSystem.Data.Repositories;

namespace HRApplicantSystem.UI.ViewModels.Applicant;

public partial class JobVacanciesViewModel : ViewModelBase
{
    private readonly IJobVacancyService _jobVacancyService;
    private readonly IApplicationService _applicationService;
    private readonly MainWindowViewModel? _mainViewModel;

    public ObservableCollection<JobVacancy> Vacancies { get; } = new();

    [ObservableProperty]
    private JobVacancy? selectedJob;

    [ObservableProperty]
    private string searchText = "";

    [ObservableProperty]
    private string message = "";

    [ObservableProperty]
    private bool hasMessage;

    public bool CanApply =>
        SelectedJob != null &&
        SelectedJob.Status == VacancyStatus.Open;

    public JobVacanciesViewModel(IJobVacancyService jobVacancyService,
                             IApplicationService applicationService, MainWindowViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        _jobVacancyService = jobVacancyService;
        _applicationService = applicationService;

        _ = LoadJobVacanciesAsync();
    }

    public async Task LoadJobVacanciesAsync()
    {
            var jobs = await _jobVacancyService.GetOpenJobsAsync();
            Vacancies.Clear();
            foreach (var job in jobs)
                Vacancies.Add(job);
    }

    partial void OnSearchTextChanged(string value)
    {
        _ = SearchAsync(value);
    }

    public async Task SearchAsync(string keyword)
    {
        var jobs = string.IsNullOrWhiteSpace(keyword)
            ? await _jobVacancyService.GetOpenJobsAsync()
            : await _jobVacancyService.SearchJobsAsync(keyword);

        Vacancies.Clear();
        foreach (var job in jobs)
            Vacancies.Add(job);
    }

    [RelayCommand]
    public async Task ApplyAsync()
    {
        if (SelectedJob == null) return;

        try
        {
            var db = new DbContext(AppConfig.ConnectionString);
            var applicantRepo = new ApplicantRepository(db);

            // get actual applicant_id from account_id
            var applicant = await applicantRepo.GetByAccountIdAsync(SessionManager.CurrentUserId ?? string.Empty);

            if (applicant == null)
            {
                Message = "Please complete your profile first!";
                HasMessage = true;
                return;
            }

            bool success = await _applicationService.SubmitApplicationAsync(
                applicant.ApplicantId,  // ← use applicant_id not account_id
                SelectedJob.VacancyId);

            Message = success ? "Application submitted successfully!" : "You may have already applied.";
            HasMessage = true;
        }
        catch (Exception ex)
        {
            Message = ex.Message;
            HasMessage = true;
        }
    }
    [RelayCommand] private void GoBack() => _mainViewModel?.NavigateToApplicantDashboard();
}