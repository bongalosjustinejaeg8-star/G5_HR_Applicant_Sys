using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using HRApplicantSystem.Data.Models;
using HRApplicantSystem.Services.Interfaces;
using HRApplicantSystem.Shared.Enums;
using HRApplicantSystem.Shared.Helpers;

namespace HRApplicantSystem.UI.ViewModels;

public class JobVacancyViewModel : ViewModelBase
{
    private readonly IJobVacancyService _jobVacancyService;
    private readonly IApplicationService _applicationService;

    public ObservableCollection<JobVacancy> FilteredVacancies { get; set; } = new();

    private JobVacancy? _selectedJob;
    public JobVacancy? SelectedJob
    {
        get => _selectedJob;
        set
        {
            _selectedJob = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanApply));
        }
    }

    private string _searchText = "";
    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            OnPropertyChanged();
            _ = SearchJobsAsync();
        }
    }

    private string _message = "";
    public string Message
    {
        get => _message;
        set { _message = value; OnPropertyChanged(); }
    }

    // Based on PDF: block apply if job is Closed
    public bool CanApply =>
        SelectedJob != null &&
        SelectedJob.Status == VacancyStatus.Open;

    public JobVacancyViewModel(IJobVacancyService jobVacancyService,
                               IApplicationService applicationService)
    {
        _jobVacancyService = jobVacancyService;
        _applicationService = applicationService;
    }

    public async Task LoadJobsAsync()
    {
        var jobs = await _jobVacancyService.GetOpenJobsAsync();
        FilteredVacancies.Clear();
        foreach (var job in jobs)
            FilteredVacancies.Add(job);
    }

    public async Task SearchJobsAsync()
    {
        var jobs = await _jobVacancyService.SearchJobsAsync(SearchText);
        FilteredVacancies.Clear();
        foreach (var job in jobs)
            FilteredVacancies.Add(job);
    }

    public async Task ApplyAsync()
    {
        if (SelectedJob == null) return;
        var applicantId = SessionManager.CurrentUserId;
        bool success = await _applicationService.SubmitApplicationAsync(applicantId!, SelectedJob.VacancyId);
        Message = success ? "Application submitted successfully!" : "Failed to submit. You may have already applied.";
    }
}