using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRApplicantSystem.Data;
using HRApplicantSystem.Data.Repositories;
using HRApplicantSystem.Shared.Enums;

namespace HRApplicantSystem.UI.ViewModels.HR;

public partial class ReportsViewModel : ViewModelBase
{
    private readonly MainWindowViewModel? _mainViewModel;

    [ObservableProperty] private ObservableCollection<dynamic> _reportItems = new();
    [ObservableProperty] private string _reportTitle = "All Applicants";
    [ObservableProperty] private string _totalCount = string.Empty;
    [ObservableProperty] private DateTimeOffset? _startDate = DateTimeOffset.Now.AddMonths(-1);
    [ObservableProperty] private DateTimeOffset? _endDate = DateTimeOffset.Now;
    public ReportsViewModel() { _ = LoadReportAsync(); }
    public ReportsViewModel(MainWindowViewModel mainViewModel) { _mainViewModel = mainViewModel; _ = LoadReportAsync(); }

    private async Task LoadReportAsync(ApplicationStatus? filter = null)
    {
        try
        {
            var db = new DbContext(AppConfig.ConnectionString);
            var appRepo = new ApplicationRepository(db);
            var applicantRepo = new ApplicantRepository(db);
            var vacancyRepo = new JobVacancyRepository(db);

            var all = await appRepo.GetAllAsync();
            var filtered = filter == null ? all : all.Where(a => a.Status == filter);

            ReportItems.Clear();
            foreach (var a in filtered)
            {
                var applicant = await applicantRepo.GetByIdAsync(a.ApplicantId);
                var vacancy = await vacancyRepo.GetByIdAsync(a.VacancyId);
                ReportItems.Add(new
                {
                    FirstName = applicant?.FullName ?? "Unknown",
                    Position = vacancy?.PositionTitle ?? "Unknown",
                    Status = a.Status.ToString(),
                    SubmittedAt = a.SubmittedAt.ToString("MMM d, yyyy")
                });
            }
            TotalCount = $"Total: {ReportItems.Count} records";
        }
        catch (Exception ex) { Console.WriteLine(ex.Message); }
    }

    [RelayCommand] private async Task GenerateReport() { await LoadReportAsync(); }
    [RelayCommand] private async Task FilterAll() { ReportTitle = "All Applicants"; await LoadReportAsync(); }
    [RelayCommand] private async Task FilterPending() { ReportTitle = "Pending"; await LoadReportAsync(ApplicationStatus.Submitted); }
    [RelayCommand] private async Task FilterAccepted() { ReportTitle = "Accepted"; await LoadReportAsync(ApplicationStatus.Accepted); }
    [RelayCommand] private async Task FilterRejected() { ReportTitle = "Rejected"; await LoadReportAsync(ApplicationStatus.Rejected); }
    [RelayCommand] private void GoBack() => _mainViewModel?.NavigateToHRDashboard();
}