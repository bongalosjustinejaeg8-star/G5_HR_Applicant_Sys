using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRApplicantSystem.Data;
using HRApplicantSystem.Data.Repositories;
using HRApplicantSystem.Shared.Helpers;

namespace HRApplicantSystem.UI.ViewModels.Applicant;

/// <summary>
/// ViewModel for the Applicant Dashboard.
/// Displays application status, upcoming interviews, and missing documents.
/// </summary>
public partial class DashboardViewModel : ViewModelBase
{
    private readonly MainWindowViewModel? _mainViewModel;

    /// <summary>
    /// Current application status display.
    /// </summary>
    [ObservableProperty]
    private string currentApplicationStatus = "Not Submitted";

    /// <summary>
    /// Date when the application was submitted.
    /// </summary>
    [ObservableProperty]
    private string applicationSubmittedDate = "N/A";

    /// <summary>
    /// Collection of missing documents for the application.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<dynamic> missingDocuments = new();

    /// <summary>
    /// Collection of upcoming interviews.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<dynamic> upcomingInterviews = new();

    /// <summary>
    /// Status or error message.
    /// </summary>
    [ObservableProperty]
    private string message = "";

    /// <summary>
    /// True while dashboard data is loading.
    /// </summary>
    [ObservableProperty]
    private bool isLoading = false;

    public DashboardViewModel()
    {
        Debug.WriteLine("[DashboardViewModel] Initializing without MainWindowViewModel");
        _ = LoadDashboardDataAsync();
    }

    public DashboardViewModel(MainWindowViewModel mainViewModel)
    {
        Debug.WriteLine("[DashboardViewModel] Initializing with MainWindowViewModel");
        _mainViewModel = mainViewModel;
        _ = LoadDashboardDataAsync();
    }

    /// <summary>
    /// Loads all dashboard data: application status, interviews, and documents.
    /// </summary>
    [RelayCommand]
    public async Task LoadDashboardDataAsync()
    {
        Debug.WriteLine("[DashboardViewModel] LoadDashboardDataAsync called");
        try
        {
            IsLoading = true;
            var db = new DbContext(AppConfig.ConnectionString);
            var applicantRepo = new ApplicantRepository(db);
            var appRepo = new ApplicationRepository(db);
            var vacancyRepo = new JobVacancyRepository(db);
            var interviewRepo = new InterviewScheduleRepository(db);

            var applicant = await applicantRepo.GetByAccountIdAsync(SessionManager.CurrentUserId ?? string.Empty);
            if (applicant == null)
            {
                CurrentApplicationStatus = "Please complete your profile first.";
                Message = "No applicant profile found.";
                Debug.WriteLine("[DashboardViewModel] Applicant not found for user");
                return;
            }

            var apps = await appRepo.GetByApplicantIdAsync(applicant.ApplicantId);
            if (apps == null || !apps.Any())
            {
                CurrentApplicationStatus = "No application submitted yet.";
                ApplicationSubmittedDate = "N/A";
                Message = "You have not submitted any applications.";
                Debug.WriteLine("[DashboardViewModel] No applications found for applicant");
                return;
            }

            var latest = apps.OrderByDescending(a => a.SubmittedAt).FirstOrDefault();

            if (latest != null)
            {
                var vacancy = await vacancyRepo.GetByIdAsync(latest.VacancyId);
                CurrentApplicationStatus = $"{latest.Status} — {vacancy?.PositionTitle ?? "Unknown"}";
                ApplicationSubmittedDate = $"Submitted: {latest.SubmittedAt:MMMM d, yyyy}";

                // Load upcoming interviews
                var interviews = await interviewRepo.GetByApplicationIdAsync(latest.ApplicationId);
                UpcomingInterviews.Clear();
                if (interviews != null)
                {
                    foreach (var interview in interviews.Where(i => i.ScheduledAt > DateTime.Now).OrderBy(i => i.ScheduledAt))
                    {
                        UpcomingInterviews.Add(new
                        {
                            ScheduledAt = interview.ScheduledAt.ToString("MMMM d, yyyy h:mm tt"),
                            Mode = interview.Mode.ToString(),
                            Location = interview.Location ?? "Online"
                        });
                    }
                }

                Message = "Dashboard data loaded successfully.";
                Debug.WriteLine($"[DashboardViewModel] Loaded data for application: {latest.ApplicationId}");
            }
            else
            {
                CurrentApplicationStatus = "No application submitted yet.";
                ApplicationSubmittedDate = "N/A";
            }
        }
        catch (Exception ex)
        {
            CurrentApplicationStatus = $"Error: {ex.Message}";
            Message = $"Error loading dashboard: {ex.Message}";
            Debug.WriteLine($"[DashboardViewModel] LoadDashboardDataAsync error: {ex}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public void GoToJobVacancies()
    {
        Debug.WriteLine("[DashboardViewModel] GoToJobVacancies called");
        _mainViewModel?.NavigateToJobVacancies();
    }

    [RelayCommand]
    public void GoToMyApplication()
    {
        Debug.WriteLine("[DashboardViewModel] GoToMyApplication called");
        _mainViewModel?.NavigateToMyApplication();
    }

    [RelayCommand]
    public void GoToMyDocuments()
    {
        Debug.WriteLine("[DashboardViewModel] GoToMyDocuments called");
        _mainViewModel?.NavigateToMyDocuments();
    }

    [RelayCommand]
    public void GoToStatusTracking()
    {
        Debug.WriteLine("[DashboardViewModel] GoToStatusTracking called");
        _mainViewModel?.NavigateToStatusTracking();
    }

    [RelayCommand]
    public void GoToProfile()
    {
        Debug.WriteLine("[DashboardViewModel] GoToProfile called");
        _mainViewModel?.NavigateToProfile();
    }

    [RelayCommand]
    public void Logout()
    {
        Debug.WriteLine("[DashboardViewModel] Logout called");
        SessionManager.Logout();
        _mainViewModel?.NavigateToLanding();
    }

    [RelayCommand]
    public void RefreshDashboard()
    {
        Debug.WriteLine("[DashboardViewModel] RefreshDashboard called");
        _ = LoadDashboardDataAsync();
    }
}
