using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRApplicantSystem.Data;
using HRApplicantSystem.Data.Repositories;
using HRApplicantSystem.Shared.Helpers;

namespace HRApplicantSystem.UI.ViewModels.Applicant;

/// <summary>
/// ViewModel for tracking the applicant's application status history.
/// Loads and displays all status changes for the current applicant's application.
/// </summary>
public partial class StatusTrackingViewModel : ViewModelBase
{
    private readonly MainWindowViewModel? _mainViewModel;

    /// <summary>
    /// Collection of status history entries for display.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<dynamic> statusHistory = new();

    /// <summary>
    /// The current/latest status of the application.
    /// </summary>
    [ObservableProperty]
    private string? currentStatus;

    /// <summary>
    /// The formatted date when the status was last updated.
    /// </summary>
    [ObservableProperty]
    private string? lastUpdated;

    /// <summary>
    /// Status or error message to display to the user.
    /// </summary>
    [ObservableProperty]
    private string message = "";

    /// <summary>
    /// True when a message should be displayed.
    /// </summary>
    [ObservableProperty]
    private bool hasMessage = false;

    /// <summary>
    /// True while data is loading from the database.
    /// </summary>
    [ObservableProperty]
    private bool isLoading = false;

    public StatusTrackingViewModel()
    {
        Debug.WriteLine("[StatusTrackingViewModel] Initializing without MainWindowViewModel");
        _ = InitializeAsync();
    }

    public StatusTrackingViewModel(MainWindowViewModel mainViewModel)
    {
        Debug.WriteLine("[StatusTrackingViewModel] Initializing with MainWindowViewModel");
        _mainViewModel = mainViewModel;
        _ = InitializeAsync();
    }

    /// <summary>
    /// Initializes the ViewModel by loading status history on construction.
    /// </summary>
    private async Task InitializeAsync()
    {
        Debug.WriteLine("[StatusTrackingViewModel] InitializeAsync called");
        try
        {
            await LoadStatusHistoryAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[StatusTrackingViewModel] InitializeAsync error: {ex.Message}");
            Message = "Failed to initialize status tracking.";
            HasMessage = true;
        }
    }

    /// <summary>
    /// Loads the application status history for the currently logged-in applicant.
    /// Fetches all status changes ordered chronologically and updates CurrentStatus and LastUpdated.
    /// </summary>
    [RelayCommand]
    public async Task LoadStatusHistoryAsync()
    {
        Debug.WriteLine("[StatusTrackingViewModel] LoadStatusHistoryAsync called");
        try
        {
            IsLoading = true;
            StatusHistory.Clear();

            // Step 1: Check if user is logged in
            var accountId = SessionManager.CurrentUserId;
            if (string.IsNullOrEmpty(accountId))
            {
                Message = "User not authenticated. Please log in.";
                HasMessage = true;
                Debug.WriteLine("[StatusTrackingViewModel] No current user ID");
                return;
            }

            var db = new DbContext(AppConfig.ConnectionString);

            // Step 2: Get the applicant record linked to this account
            var applicantRepo = new ApplicantRepository(db);
            var applicant = await applicantRepo.GetByAccountIdAsync(accountId);

            if (applicant == null)
            {
                Message = "Applicant profile not found.";
                HasMessage = true;
                Debug.WriteLine("[StatusTrackingViewModel] Applicant not found for account");
                return;
            }

            // Step 3: Get the applicant's application
            var applicationRepo = new ApplicationRepository(db);
            var applications = await applicationRepo.GetByApplicantIdAsync(applicant.ApplicantId);
            var application = System.Linq.Enumerable.FirstOrDefault(applications);

            if (application == null)
            {
                Message = "No application found. Please submit an application first.";
                HasMessage = true;
                Debug.WriteLine("[StatusTrackingViewModel] No application found for applicant");
                return;
            }

            // Step 4: Load the status history for that application
            var historyRepo = new ApplicationStatusHistoryRepository(db);
            var history = await historyRepo.GetByApplicationIdAsync(application.ApplicationId);

            // Step 5: Populate the StatusHistory collection for binding
            foreach (var entry in history)
            {
                StatusHistory.Add(new
                {
                    Status = entry.NewStatus.ToString(),
                    OldStatus = entry.OldStatus?.ToString() ?? "—",
                    ChangedBy = entry.ChangedBy ?? "System",
                    ChangedAt = entry.ChangedAt.ToString("MMMM d, yyyy h:mm tt"),
                    Remarks = entry.Remarks ?? "—"
                });
            }

            // Step 6: Set the current/latest status and last updated time
            var latest = System.Linq.Enumerable.LastOrDefault(history);
            if (latest != null)
            {
                CurrentStatus = latest.NewStatus.ToString();
                LastUpdated = latest.ChangedAt.ToString("MMMM d, yyyy h:mm tt");
            }
            else
            {
                CurrentStatus = application.Status.ToString();
                LastUpdated = application.SubmittedAt.ToString("MMMM d, yyyy h:mm tt");
            }

            Debug.WriteLine($"[StatusTrackingViewModel] Loaded {StatusHistory.Count} history entries. Current: {CurrentStatus}");

            if (StatusHistory.Count == 0)
            {
                Message = "No status history found yet.";
                HasMessage = true;
            }
        }
        catch (Exception ex)
        {
            Message = $"Error loading status history: {ex.Message}";
            HasMessage = true;
            Debug.WriteLine($"[StatusTrackingViewModel] LoadStatusHistoryAsync error: {ex}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Navigates back to the applicant dashboard.
    /// </summary>
    [RelayCommand]
    public void GoBack()
    {
        Debug.WriteLine("[StatusTrackingViewModel] GoBack called");
        _mainViewModel?.NavigateToApplicantDashboard();
    }
}
