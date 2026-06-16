using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRApplicantSystem.Data;
using HRApplicantSystem.Data.Repositories;
using HRApplicantSystem.Shared.Enums;
using HRApplicantSystem.Shared.Helpers;

namespace HRApplicantSystem.UI.ViewModels.HR;

public partial class HRDashboardViewModel : ViewModelBase
{
    private readonly MainWindowViewModel? _mainViewModel;

    [ObservableProperty] private int totalApplicants = 0;
    [ObservableProperty] private int pendingReview = 0;
    [ObservableProperty] private int shortlisted = 0;
    [ObservableProperty] private int accepted = 0;
    [ObservableProperty] private int rejected = 0;
    [ObservableProperty] private int forFinalReview = 0;
    [ObservableProperty] private string welcomeMessage = $"Welcome, {SessionManager.CurrentUserName}!";
    [ObservableProperty] private string message = "";
    [ObservableProperty] private bool isLoading = false;
    [ObservableProperty] private ObservableCollection<dynamic> recentActivities = new();
    [ObservableProperty] private ObservableCollection<dynamic> pendingDecisions = new();

    public HRDashboardViewModel()
    {
        _ = LoadDashboardAsync();
    }

    public HRDashboardViewModel(MainWindowViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        _ = LoadDashboardAsync();
    }

    [RelayCommand]
    private async Task LoadDashboardAsync()
    {
        try
        {
            IsLoading = true;
            var db = new DbContext(AppConfig.ConnectionString);
            var appRepo = new ApplicationRepository(db);
            var applicantRepo = new ApplicantRepository(db);
            var vacancyRepo = new JobVacancyRepository(db);
            var historyRepo = new ApplicationStatusHistoryRepository(db);

            var all = (await appRepo.GetAllAsync()).ToList();

            // stats
            TotalApplicants = all.Count;
            PendingReview = all.Count(a => a.Status == ApplicationStatus.Submitted);
            Shortlisted = all.Count(a => a.Status == ApplicationStatus.Shortlisted);
            Accepted = all.Count(a => a.Status == ApplicationStatus.Accepted);
            Rejected = all.Count(a => a.Status == ApplicationStatus.Rejected);
            ForFinalReview = all.Count(a => a.Status == ApplicationStatus.ForFinalReview);

            // pending hiring decisions — ForFinalReview apps
            PendingDecisions.Clear();
            var forDecision = all.Where(a =>
                a.Status == ApplicationStatus.ForFinalReview ||
                a.Status == ApplicationStatus.Shortlisted ||
                a.Status == ApplicationStatus.ForInterview);

            foreach (var app in forDecision.Take(5))
            {
                var applicant = await applicantRepo.GetByIdAsync(app.ApplicantId);
                var vacancy = await vacancyRepo.GetByIdAsync(app.VacancyId);
                PendingDecisions.Add(new
                {
                    ApplicationId = app.ApplicationId,
                    FullName = applicant?.FullName ?? "Unknown",
                    Position = vacancy?.PositionTitle ?? "Unknown",
                    Status = app.Status.ToString()
                });
            }

            // recent activities from status history
            RecentActivities.Clear();
            var recentApps = all.OrderByDescending(a => a.SubmittedAt).Take(8);
            foreach (var app in recentApps)
            {
                var applicant = await applicantRepo.GetByIdAsync(app.ApplicantId);
                var vacancy = await vacancyRepo.GetByIdAsync(app.VacancyId);
                RecentActivities.Add(new
                {
                    Description = $"{applicant?.FullName ?? "Unknown"} — {vacancy?.PositionTitle ?? "Unknown"}",
                    Status = app.Status.ToString(),
                    Timestamp = app.SubmittedAt.ToString("MMM d, yyyy"),
                    IsLocked = app.IsLocked
                });
            }
        }
        catch (Exception ex)
        {
            Message = $"Error loading dashboard: {ex.Message}";
            Debug.WriteLine($"[HRDashboardViewModel] Error: {ex}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    // navigation
    [RelayCommand] public void GoToApplicantList() => _mainViewModel?.NavigateToApplicantList();
    [RelayCommand] public void GoToJobVacancyMgmt() => _mainViewModel?.NavigateToJobVacancyMgmt();
    [RelayCommand] public void GoToReports() => _mainViewModel?.NavigateToReports();
    [RelayCommand] public void GoToMaintenance() => _mainViewModel?.NavigateToMaintenance();
    [RelayCommand] public void GoToHiringDecision() => _mainViewModel?.NavigateToHiringDecision();
    [RelayCommand] public void Refresh() => _ = LoadDashboardAsync();
    [RelayCommand] public void Logout() { SessionManager.Logout(); _mainViewModel?.NavigateToLanding(); }
}