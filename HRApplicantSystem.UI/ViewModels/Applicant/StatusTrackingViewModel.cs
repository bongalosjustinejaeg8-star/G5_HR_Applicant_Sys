using System.Collections.ObjectModel;
using HRApplicantSystem.Shared.Helpers;

namespace HRApplicantSystem.UI.ViewModels;

public class StatusTrackingViewModel : ViewModelBase
{
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

    public StatusTrackingViewModel()
    {
        // TODO: Initialize status tracking
    }

    public async Task LoadStatusHistoryAsync()
    {
        // TODO: Load application status history
    }
}
