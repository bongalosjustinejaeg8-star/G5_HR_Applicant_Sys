using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using HRApplicantSystem.Data;
using HRApplicantSystem.Data.Repositories;
using HRApplicantSystem.Shared.Helpers;

namespace HRApplicantSystem.UI.ViewModels.Applicant;

public partial class StatusTrackingViewModel : ViewModelBase
{
    private readonly MainWindowViewModel? _mainViewModel;

    public ObservableCollection<dynamic> StatusHistory { get; set; } = new();

    private string? _currentStatus;
    public string? CurrentStatus
    {
        get => _currentStatus;
        set { _currentStatus = value; OnPropertyChanged(); }
    }

    private string? _lastUpdated;
    public string? LastUpdated
    {
        get => _lastUpdated;
        set { _lastUpdated = value; OnPropertyChanged(); }
    }

    public StatusTrackingViewModel() { _ = LoadStatusHistoryAsync(); }
    public StatusTrackingViewModel(MainWindowViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        _ = LoadStatusHistoryAsync();
    }

    public async Task LoadStatusHistoryAsync()
    {
        try
        {
            var db = new DbContext(AppConfig.ConnectionString);
            var applicantRepo = new ApplicantRepository(db);
            var appRepo = new ApplicationRepository(db);
            var historyRepo = new ApplicationStatusHistoryRepository(db);

            var applicant = await applicantRepo.GetByAccountIdAsync(SessionManager.CurrentUserId ?? string.Empty);
            if (applicant == null) return;

            var apps = await appRepo.GetByApplicantIdAsync(applicant.ApplicantId);
            var latest = apps.OrderByDescending(a => a.SubmittedAt).FirstOrDefault();
            if (latest == null) return;

            CurrentStatus = latest.Status.ToString();
            LastUpdated = latest.SubmittedAt.ToString("MMMM d, yyyy h:mm tt");

            var history = await historyRepo.GetByApplicationIdAsync(latest.ApplicationId);
            StatusHistory.Clear();
            foreach (var h in history)
            {
                StatusHistory.Add(new
                {
                    StatusName = h.NewStatus.ToString(),
                    ChangedDate = h.ChangedAt.ToString("MMMM d, yyyy"),
                    ChangedTime = h.ChangedAt.ToString("h:mm tt"),
                    Details = h.Remarks ?? "Status updated",
                    ChangedBy = h.ChangedBy ?? "System"
                });
            }
        }
        catch (Exception ex) { Console.WriteLine(ex.Message); }
    }

    [RelayCommand] private void GoBack() => _mainViewModel?.NavigateToApplicantDashboard();
}