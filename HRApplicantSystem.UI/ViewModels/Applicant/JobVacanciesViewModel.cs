using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRApplicantSystem.Data.Models;
using HRApplicantSystem.Services.Interfaces;
using HRApplicantSystem.Shared.Helpers;
using HRApplicantSystem.Shared.Enums;

namespace HRApplicantSystem.UI.ViewModels.Applicant;

public partial class JobVacanciesViewModel : ViewModelBase
{
    private readonly IJobVacancyService _jobVacancyService;
    private readonly IApplicationService _applicationService;

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

    public JobVacanciesViewModel(
        IJobVacancyService jobVacancyService,
        IApplicationService applicationService)
    {
        _jobVacancyService = jobVacancyService;
        _applicationService = applicationService;

        Message = "VM LOADED";
        HasMessage = true;

        _ = LoadJobVacanciesAsync();
    }

    public async Task LoadJobVacanciesAsync()
    {
        var jobs = await _jobVacancyService.SearchJobsAsync(""); // or even better: ALL jobs

        Message = $"DEBUG: {jobs.Count()} jobs loaded";
        HasMessage = true;

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
        if (SelectedJob == null)
            return;

        var applicantId = SessionManager.CurrentUserId;

        if (string.IsNullOrEmpty(applicantId))
        {
            Message = "Please log in first.";
            HasMessage = true;
            return;
        }

        bool success = await _applicationService.SubmitApplicationAsync(
            applicantId,
            SelectedJob.VacancyId);

        Message = success
            ? "Application submitted successfully!"
            : "You may have already applied.";

        HasMessage = true;
    }
}