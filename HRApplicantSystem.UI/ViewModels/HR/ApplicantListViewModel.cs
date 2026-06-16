using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRApplicantSystem.Data;
using HRApplicantSystem.Data.Models;
using HRApplicantSystem.Data.Repositories;
using HRApplicantSystem.Shared.Enums;
using HRApplicantSystem.Shared.Helpers;

namespace HRApplicantSystem.UI.ViewModels.HR;

public partial class ApplicantListViewModel : ViewModelBase
{
    private readonly MainWindowViewModel? _mainViewModel;

    [ObservableProperty] private ObservableCollection<dynamic> _applicants = new();
    [ObservableProperty] private object? _selectedApplicant;
    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private bool _hasSelected = false;
    [ObservableProperty] private string _message = string.Empty;
    [ObservableProperty] private bool _hasMessage = false;
    [ObservableProperty] private bool _selectedIsLocked = false;
    [ObservableProperty] private string _lockStatus = string.Empty;

    public ApplicantListViewModel() { _ = LoadApplicantsAsync(); }
    public ApplicantListViewModel(MainWindowViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        _ = LoadApplicantsAsync();
    }

    partial void OnSelectedApplicantChanged(object? value)
    {
        HasSelected = value != null;
        if (value == null) { LockStatus = string.Empty; return; }

        // show edit lock status when an application is selected
        var isLocked = ((dynamic)value).IsLocked as bool? ?? false;
        SelectedIsLocked = isLocked;
        LockStatus = isLocked
            ? "🔒 Locked — applicant cannot edit this application."
            : "✅ Unlocked — applicant can still edit.";
    }

    private async Task LoadApplicantsAsync()
    {
        try
        {
            var db = new DbContext(AppConfig.ConnectionString);
            var appRepo = new ApplicationRepository(db);
            var applicantRepo = new ApplicantRepository(db);
            var vacancyRepo = new JobVacancyRepository(db);

            var apps = await appRepo.GetAllAsync();
            Applicants.Clear();
            foreach (var app in apps)
            {
                var applicant = await applicantRepo.GetByIdAsync(app.ApplicantId);
                var vacancy = await vacancyRepo.GetByIdAsync(app.VacancyId);
                Applicants.Add(new
                {
                    ApplicationId = app.ApplicationId,
                    FullName = applicant?.FullName ?? "Unknown",
                    Position = vacancy?.PositionTitle ?? "Unknown",
                    Status = app.Status.ToString(),
                    IsLocked = app.IsLocked,
                    SubmittedAt = app.SubmittedAt.ToString("MMM d, yyyy")
                });
            }
        }
        catch (Exception ex)
        {
            Message = ex.Message;
            HasMessage = true;
        }
    }

    [RelayCommand]
    private async Task StartReview()
    {
        if (SelectedApplicant == null) return;
        try
        {
            var appId = ((dynamic)SelectedApplicant).ApplicationId as string ?? string.Empty;
            var isLocked = ((dynamic)SelectedApplicant).IsLocked as bool? ?? false;

            // demonstrate edit lock — if already locked, inform HR
            if (isLocked)
            {
                Message = "⚠️ This application is already locked and under review.";
                HasMessage = true;
                return;
            }

            var db = new DbContext(AppConfig.ConnectionString);
            var appRepo = new ApplicationRepository(db);
            var historyRepo = new ApplicationStatusHistoryRepository(db);

            // lock the application — applicant can no longer edit
            await appRepo.LockAsync(appId);
            await appRepo.UpdateStatusAsync(appId, ApplicationStatus.UnderReview);
            await historyRepo.CreateAsync(new ApplicationStatusHistory
            {
                ApplicationId = appId,
                ChangedBy = SessionManager.CurrentUserId,
                OldStatus = ApplicationStatus.Submitted,
                NewStatus = ApplicationStatus.UnderReview,
                Remarks = "HR started review — application locked"
            });

            Message = "✅ Review started. Application is now locked for editing.";
            HasMessage = true;
            await LoadApplicantsAsync();
        }
        catch (Exception ex)
        {
            Message = ex.Message;
            HasMessage = true;
        }
    }

    [RelayCommand]
    private async Task Shortlist()
    {
        if (SelectedApplicant == null) return;
        try
        {
            var appId = ((dynamic)SelectedApplicant).ApplicationId as string ?? string.Empty;
            var db = new DbContext(AppConfig.ConnectionString);
            var appRepo = new ApplicationRepository(db);
            var historyRepo = new ApplicationStatusHistoryRepository(db);
            var app = await appRepo.GetByIdAsync(appId);
            if (app == null) return;

            await appRepo.UpdateStatusAsync(appId, ApplicationStatus.Shortlisted);
            await historyRepo.CreateAsync(new ApplicationStatusHistory
            {
                ApplicationId = appId,
                ChangedBy = SessionManager.CurrentUserId,
                OldStatus = app.Status,
                NewStatus = ApplicationStatus.Shortlisted,
                Remarks = "Applicant shortlisted by HR"
            });

            Message = "✅ Applicant shortlisted.";
            HasMessage = true;
            await LoadApplicantsAsync();
        }
        catch (Exception ex) { Message = ex.Message; HasMessage = true; }
    }

    [RelayCommand]
    private async Task MoveToFinalReview()
    {
        if (SelectedApplicant == null) return;
        try
        {
            var appId = ((dynamic)SelectedApplicant).ApplicationId as string ?? string.Empty;
            var db = new DbContext(AppConfig.ConnectionString);
            var appRepo = new ApplicationRepository(db);
            var historyRepo = new ApplicationStatusHistoryRepository(db);
            var app = await appRepo.GetByIdAsync(appId);
            if (app == null) return;

            await appRepo.UpdateStatusAsync(appId, ApplicationStatus.ForFinalReview);
            await historyRepo.CreateAsync(new ApplicationStatusHistory
            {
                ApplicationId = appId,
                ChangedBy = SessionManager.CurrentUserId,
                OldStatus = app.Status,
                NewStatus = ApplicationStatus.ForFinalReview,
                Remarks = "Moved to final review"
            });

            Message = "✅ Moved to final review.";
            HasMessage = true;
            await LoadApplicantsAsync();
        }
        catch (Exception ex) { Message = ex.Message; HasMessage = true; }
    }

    [RelayCommand] private void GoBack() => _mainViewModel?.NavigateToHRDashboard();

    
}

    