using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using HRApplicantSystem.Data;
using HRApplicantSystem.Data.Repositories;
using HRApplicantSystem.Shared.Helpers;

namespace HRApplicantSystem.UI.ViewModels.Applicant;

public partial class DashboardViewModel : ViewModelBase
{
    private readonly MainWindowViewModel? _mainViewModel;

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

    public DashboardViewModel() { }

    public DashboardViewModel(MainWindowViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        _ = LoadDashboardDataAsync();
    }

    public async Task LoadDashboardDataAsync()
    {
        try
        {
            var db = new DbContext(AppConfig.ConnectionString);
            var applicantRepo = new ApplicantRepository(db);
            var appRepo = new ApplicationRepository(db);
            var vacancyRepo = new JobVacancyRepository(db);

            var applicant = await applicantRepo.GetByAccountIdAsync(SessionManager.CurrentUserId ?? string.Empty);
            if (applicant == null) { CurrentApplicationStatus = "Please complete your profile first."; return; }

            var apps = await appRepo.GetByApplicantIdAsync(applicant.ApplicantId);
            var latest = apps.OrderByDescending(a => a.SubmittedAt).FirstOrDefault();

            if (latest != null)
            {
                var vacancy = await vacancyRepo.GetByIdAsync(latest.VacancyId);
                CurrentApplicationStatus = $"{latest.Status} — {vacancy?.PositionTitle ?? "Unknown"}";
                ApplicationSubmittedDate = $"Submitted: {latest.SubmittedAt:MMMM d, yyyy}";
            }
            else
            {
                CurrentApplicationStatus = "No application submitted yet.";
            }
        }
        catch (Exception ex) { CurrentApplicationStatus = $"Error: {ex.Message}"; }
    }

    [RelayCommand] public void GoToJobVacancies() => _mainViewModel?.NavigateToJobVacancies();
    [RelayCommand] public void GoToMyApplication() => _mainViewModel?.NavigateToMyApplication();
    [RelayCommand] public void GoToMyDocuments() => _mainViewModel?.NavigateToMyDocuments();
    [RelayCommand] public void GoToStatusTracking() => _mainViewModel?.NavigateToStatusTracking();
    [RelayCommand] public void GoToProfile() => _mainViewModel?.NavigateToProfile();
    [RelayCommand] public void Logout() { SessionManager.Logout(); _mainViewModel?.NavigateToLanding(); }
}