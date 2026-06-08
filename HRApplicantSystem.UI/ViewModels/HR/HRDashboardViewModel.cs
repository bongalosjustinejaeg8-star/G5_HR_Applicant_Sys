using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using HRApplicantSystem.Shared.Helpers;

namespace HRApplicantSystem.UI.ViewModels;

public class HRDashboardViewModel : ViewModelBase
{
    private int _totalApplicants;
    public int TotalApplicants
    {
        get => _totalApplicants;
        set { _totalApplicants = value; OnPropertyChanged(); }
    }

    private int _screeningCount;
    public int ScreeningCount
    {
        get => _screeningCount;
        set { _screeningCount = value; OnPropertyChanged(); }
    }

    private int _interviewCount;
    public int InterviewCount
    {
        get => _interviewCount;
        set { _interviewCount = value; OnPropertyChanged(); }
    }

    private int _hiredCount;
    public int HiredCount
    {
        get => _hiredCount;
        set { _hiredCount = value; OnPropertyChanged(); }
    }

    public ObservableCollection<dynamic> RecentActivities { get; set; } = new();

    public HRDashboardViewModel()
    {
        // TODO: Load recruitment summary data from service
    }

    public async Task LoadDashboardDataAsync()
    {
        // TODO: Implement loading of dashboard statistics
    }
}
