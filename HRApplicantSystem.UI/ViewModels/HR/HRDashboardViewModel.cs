using System;
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

    [ObservableProperty] private int _totalApplicants = 0;
    [ObservableProperty] private int _pendingReview = 0;
    [ObservableProperty] private int _shortlisted = 0;
    [ObservableProperty] private int _accepted = 0;
    [ObservableProperty] private string _welcomeMessage = $"Welcome, {SessionManager.CurrentUserName}!";

    public HRDashboardViewModel() { _ = LoadStatsAsync(); }

    public HRDashboardViewModel(MainWindowViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        _ = LoadStatsAsync();
    }

    private async Task LoadStatsAsync()
    {
        try
        {
            var db = new DbContext(AppConfig.ConnectionString);
            var repo = new ApplicationRepository(db);
            var all = (await repo.GetAllAsync()).ToList();
            TotalApplicants = all.Count;
            PendingReview = all.Count(a => a.Status == ApplicationStatus.Submitted);
            Shortlisted = all.Count(a => a.Status == ApplicationStatus.Shortlisted);
            Accepted = all.Count(a => a.Status == ApplicationStatus.Accepted);
        }
        catch (Exception ex) { Console.WriteLine(ex.Message); }
    }

    [RelayCommand] public void GoToApplicantList() => _mainViewModel?.NavigateToApplicantList();
    [RelayCommand] public void GoToJobVacancyMgmt() => _mainViewModel?.NavigateToJobVacancyMgmt();
    [RelayCommand] public void GoToReports() => _mainViewModel?.NavigateToReports();
    [RelayCommand] public void GoToMaintenance() => _mainViewModel?.NavigateToMaintenance();
    [RelayCommand] public void Logout() { SessionManager.Logout(); _mainViewModel?.NavigateToLanding(); }
}