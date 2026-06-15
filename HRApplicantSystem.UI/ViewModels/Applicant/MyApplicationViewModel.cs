using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRApplicantSystem.Data;
using HRApplicantSystem.Data.Models;
using HRApplicantSystem.Data.Repositories;
using HRApplicantSystem.Services.Interfaces;
using HRApplicantSystem.Shared.Enums;
using HRApplicantSystem.Shared.Helpers;

namespace HRApplicantSystem.UI.ViewModels.Applicant;

public partial class MyApplicationViewModel : ViewModelBase
{
    private readonly MainWindowViewModel? _mainViewModel;
    private readonly IApplicationService _applicationService;

    public ObservableCollection<Application> Applications { get; set; } = new();

    private Application? _selectedApplication;
    public Application? SelectedApplication
    {
        get => _selectedApplication;
        set { _selectedApplication = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsEditable)); OnPropertyChanged(nameof(IsLocked)); }
    }

    [ObservableProperty] private string _positionTitle = string.Empty;
    [ObservableProperty] private string _currentStatus = string.Empty;
    [ObservableProperty] private string _submittedAt = string.Empty;
    [ObservableProperty] private string _isLockedMessage = string.Empty;
    [ObservableProperty] private string _message = string.Empty;
    [ObservableProperty] private bool _hasMessage = false;

    public bool IsEditable => SelectedApplication?.Status == ApplicationStatus.Draft || SelectedApplication?.Status == ApplicationStatus.Submitted;
    public bool IsLocked => SelectedApplication != null && !IsEditable;

    public MyApplicationViewModel(IApplicationService applicationService)
    {
        _applicationService = applicationService;
        _ = LoadApplicationsAsync();
    }

    public MyApplicationViewModel(MainWindowViewModel mainViewModel, IApplicationService applicationService)
    {
        _mainViewModel = mainViewModel;
        _applicationService = applicationService;
        _ = LoadApplicationsAsync();
    }

    public async Task LoadApplicationsAsync()
    {
        try
        {
            var db = new DbContext(AppConfig.ConnectionString);
            var applicantRepo = new ApplicantRepository(db);
            var vacancyRepo = new JobVacancyRepository(db);
            var applicant = await applicantRepo.GetByAccountIdAsync(SessionManager.CurrentUserId ?? string.Empty);
            if (applicant == null) { Message = "Please complete your profile first."; HasMessage = true; return; }

            var apps = await _applicationService.GetByApplicantIdAsync(applicant.ApplicantId);
            Applications.Clear();
            foreach (var app in apps) Applications.Add(app);

            var latest = apps.OrderByDescending(a => a.SubmittedAt).FirstOrDefault();
            if (latest != null)
            {
                SelectedApplication = latest;
                var vacancy = await vacancyRepo.GetByIdAsync(latest.VacancyId);
                PositionTitle = vacancy?.PositionTitle ?? "Unknown Position";
                CurrentStatus = latest.Status.ToString();
                SubmittedAt = $"Submitted: {latest.SubmittedAt:MMMM d, yyyy}";
                IsLockedMessage = latest.IsLocked ? "⚠️ Application is locked — HR is reviewing." : "✅ Application is editable.";
            }
            else
            {
                Message = "No application found.";
                HasMessage = true;
            }
        }
        catch (Exception ex) { Message = ex.Message; HasMessage = true; }
    }

    public async Task SubmitApplicationAsync()
    {
        if (SelectedApplication == null) return;
        try
        {
            var db = new DbContext(AppConfig.ConnectionString);
            var applicantRepo = new ApplicantRepository(db);
            var applicant = await applicantRepo.GetByAccountIdAsync(SessionManager.CurrentUserId?? string.Empty);
            if (applicant == null) return;
            bool success = await _applicationService.SubmitApplicationAsync(applicant.ApplicantId, SelectedApplication.VacancyId);
            Message = success ? "Application submitted!" : "Failed to submit.";
            HasMessage = true;
            await LoadApplicationsAsync();
        }
        catch (Exception ex) { Message = ex.Message; HasMessage = true; }
    }

    [RelayCommand] private void GoBack() => _mainViewModel?.NavigateToApplicantDashboard();
}