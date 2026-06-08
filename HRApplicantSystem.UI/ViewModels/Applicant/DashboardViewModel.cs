using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using HRApplicantSystem.Shared.Helpers;

namespace HRApplicantSystem.UI.ViewModels;

public class DashboardViewModel : ViewModelBase
{
    private string? _currentApplicationStatus;
    public string? CurrentApplicationStatus
    {
        get => _currentApplicationStatus;
        set { _currentApplicationStatus = value; OnPropertyChanged(); }
    }

    private string? _applicationSubmittedDate;
    public string? ApplicationSubmittedDate
    {
        get => _applicationSubmittedDate;
        set { _applicationSubmittedDate = value; OnPropertyChanged(); }
    }

    public ObservableCollection<dynamic> MissingDocuments { get; set; } = new();
    public ObservableCollection<dynamic> UpcomingInterviews { get; set; } = new();

    public DashboardViewModel()
    {
        // TODO: Initialize dashboard
    }

    public async Task LoadDashboardDataAsync()
    {
        // TODO: Load dashboard information
    }
}
