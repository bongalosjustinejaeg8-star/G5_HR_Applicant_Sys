using System;
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

/// <summary>
/// ViewModel for the HR Dashboard.
/// Displays key metrics and provides navigation to HR modules.
/// </summary>
public partial class HRDashboardViewModel : ViewModelBase
{
    private readonly MainWindowViewModel? _mainViewModel;

    /// <summary>
    /// Total number of applicants in the system.
    /// </summary>
    [ObservableProperty]
    private int totalApplicants = 0;

    /// <summary>
    /// Number of applications pending review.
    /// </summary>
    [ObservableProperty]
    private int pendingReview = 0;

    /// <summary>
    /// Number of shortlisted applicants.
    /// </summary>
    [ObservableProperty]
    private int shortlisted = 0;

    /// <summary>
    /// Number of accepted applicants.
    /// </summary>
    [ObservableProperty]
    private int accepted = 0;

    /// <summary>
    /// Welcome message with HR staff name.
    /// </summary>
    [ObservableProperty]
    private string welcomeMessage = $"Welcome, {SessionManager.CurrentUserName}!";

    /// <summary>
    /// Status or error message.
    /// </summary>
    [ObservableProperty]
    private string message = "";

    /// <summary>
    /// True while statistics are loading.
    /// </summary>
    [ObservableProperty]
    private bool isLoading = false;

    public HRDashboardViewModel()
    {
        Debug.WriteLine("[HRDashboardViewModel] Initializing without MainWindowViewModel");
        _ = LoadStatsAsync();
    }

    public HRDashboardViewModel(MainWindowViewModel mainViewModel)
    {
        Debug.WriteLine("[HRDashboardViewModel] Initializing with MainWindowViewModel");
        _mainViewModel = mainViewModel;
        _ = LoadStatsAsync();
    }

    /// <summary>
    /// Loads dashboard statistics from the database.
    /// </summary>
    [RelayCommand]
    private async Task LoadStatsAsync()
    {
        Debug.WriteLine("[HRDashboardViewModel] LoadStatsAsync called");
        try
        {
            IsLoading = true;
            var db = new DbContext(AppConfig.ConnectionString);
            var repo = new ApplicationRepository(db);
            var all = (await repo.GetAllAsync()).ToList();
            
            TotalApplicants = all.Count;
            PendingReview = all.Count(a => a.Status == ApplicationStatus.Submitted);
            Shortlisted = all.Count(a => a.Status == ApplicationStatus.Shortlisted);
            Accepted = all.Count(a => a.Status == ApplicationStatus.Accepted);

            Message = "Dashboard statistics loaded successfully.";
            Debug.WriteLine($"[HRDashboardViewModel] Stats loaded: Total={TotalApplicants}, Pending={PendingReview}, Shortlisted={Shortlisted}, Accepted={Accepted}");
        }
        catch (Exception ex)
        {
            Message = $"Error loading statistics: {ex.Message}";
            Debug.WriteLine($"[HRDashboardViewModel] LoadStatsAsync error: {ex}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public void GoToApplicantList()
    {
        Debug.WriteLine("[HRDashboardViewModel] GoToApplicantList called");
        _mainViewModel?.NavigateToApplicantList();
    }

    [RelayCommand]
    public void GoToJobVacancyMgmt()
    {
        Debug.WriteLine("[HRDashboardViewModel] GoToJobVacancyMgmt called");
        _mainViewModel?.NavigateToJobVacancyMgmt();
    }

    [RelayCommand]
    public void GoToReports()
    {
        Debug.WriteLine("[HRDashboardViewModel] GoToReports called");
        _mainViewModel?.NavigateToReports();
    }

    [RelayCommand]
    public void GoToMaintenance()
    {
        Debug.WriteLine("[HRDashboardViewModel] GoToMaintenance called");
        _mainViewModel?.NavigateToMaintenance();
    }

    [RelayCommand]
    public void Logout()
    {
        Debug.WriteLine("[HRDashboardViewModel] Logout called");
        SessionManager.Logout();
        _mainViewModel?.NavigateToLanding();
    }

    [RelayCommand]
    public void RefreshStats()
    {
        Debug.WriteLine("[HRDashboardViewModel] RefreshStats called");
        _ = LoadStatsAsync();
    }
}
