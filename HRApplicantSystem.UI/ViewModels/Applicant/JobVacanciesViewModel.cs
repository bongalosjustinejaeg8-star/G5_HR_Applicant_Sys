using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRApplicantSystem.Data.Models;
using HRApplicantSystem.Services.Interfaces;
using HRApplicantSystem.Shared.Helpers;
using HRApplicantSystem.Shared.Enums;
using HRApplicantSystem.Data;
using HRApplicantSystem.Data.Repositories;
using System;

namespace HRApplicantSystem.UI.ViewModels.Applicant;

public partial class JobVacanciesViewModel : ViewModelBase
{
    private readonly IJobVacancyService _jobVacancyService;
    private readonly IApplicationService _applicationService;
    private readonly MainWindowViewModel? _mainViewModel;

    public ObservableCollection<JobVacancy> Vacancies { get; } = new();

    [ObservableProperty] private JobVacancy? selectedJob;
    [ObservableProperty] private string searchText = "";
    [ObservableProperty] private string message = "";
    [ObservableProperty] private bool hasMessage;
    [ObservableProperty] private bool canApply;

    public JobVacanciesViewModel(
        IJobVacancyService jobVacancyService,
        IApplicationService applicationService,
        MainWindowViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        _jobVacancyService = jobVacancyService;
        _applicationService = applicationService;
        _ = LoadJobVacanciesAsync();
    }

    public async Task LoadJobVacanciesAsync()
    {
        // fetch all open vacancies and populate the list
        var jobs = await _jobVacancyService.GetOpenJobsAsync();
        Vacancies.Clear();
        foreach (var job in jobs)
            Vacancies.Add(job);
    }

    partial void OnSearchTextChanged(string value)
    {
        // trigger search whenever the search box changes
        _ = SearchAsync(value);
    }

    public async Task SearchAsync(string keyword)
    {
        // empty keyword reloads all open jobs; otherwise filter by keyword
        var jobs = string.IsNullOrWhiteSpace(keyword)
            ? await _jobVacancyService.GetOpenJobsAsync()
            : await _jobVacancyService.SearchJobsAsync(keyword);

        Vacancies.Clear();
        foreach (var job in jobs)
            Vacancies.Add(job);
    }

    partial void OnSelectedJobChanged(JobVacancy? value)
    {
        // enable the apply button whenever a job is selected
        CanApply = true;
        OnPropertyChanged(nameof(CanApply));
    }

    [RelayCommand]
    public async Task ApplyAsync()
    {
        if (SelectedJob == null) return;

        try
        {
            var db = new DbContext(AppConfig.ConnectionString);
            var applicantRepo = new ApplicantRepository(db);

            // look up the applicant profile linked to the current session account
            var applicant = await applicantRepo.GetByAccountIdAsync(
                SessionManager.CurrentUserId ?? string.Empty);

            if (applicant == null)
            {
                Message = "Please complete your profile first!";
                HasMessage = true;
                return;
            }

            // submit — no userId passed since applicants are not in the Users table
            bool success = await _applicationService.SubmitApplicationAsync(
                applicant.ApplicantId,
                SelectedJob.VacancyId);

            Message = success
                ? "Application submitted successfully!"
                : "You may have already applied or job is closed.";

            HasMessage = true;
        }
        catch (Exception ex)
        {
            Message = ex.Message;
            Console.WriteLine(Message);
            HasMessage = true;
        }
    }

    [RelayCommand]
    private void GoBack() => _mainViewModel?.NavigateToApplicantDashboard();
}