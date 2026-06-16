using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
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
    [ObservableProperty] private string _reportTitle = "All applicants";
    [ObservableProperty] private string _totalCount = string.Empty;
    [ObservableProperty] private string _message = string.Empty;
    [ObservableProperty] private bool _hasMessage = false;
    [ObservableProperty] private bool _isLoading = false;
    [ObservableProperty] private DateTimeOffset? _startDate = DateTimeOffset.Now.AddMonths(-1);
    [ObservableProperty] private DateTimeOffset? _endDate = DateTimeOffset.Now;

    private ApplicationStatus? _currentFilter = null;

    public ReportsViewModel() { _ = LoadReportAsync(); }
    public ReportsViewModel(MainWindowViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        _ = LoadReportAsync();
    }

    private async Task LoadReportAsync(ApplicationStatus? filter = null)
    {
        try
        {
            IsLoading = true;
            _currentFilter = filter;

            var db = new DbContext(AppConfig.ConnectionString);
            var appRepo = new ApplicationRepository(db);
            var applicantRepo = new ApplicantRepository(db);
            var vacancyRepo = new JobVacancyRepository(db);

            var all = await appRepo.GetAllAsync();

            // apply status filter
            var filtered = filter == null ? all : all.Where(a => a.Status == filter);

            // apply date filter
            if (StartDate.HasValue)
                filtered = filtered.Where(a => a.SubmittedAt >= StartDate.Value.DateTime);
            if (EndDate.HasValue)
                filtered = filtered.Where(a => a.SubmittedAt <= EndDate.Value.DateTime.AddDays(1));

            ReportItems.Clear();
            foreach (var a in filtered)
            {
                var applicant = await applicantRepo.GetByIdAsync(a.ApplicantId);
                var vacancy = await vacancyRepo.GetByIdAsync(a.VacancyId);
                ReportItems.Add(new
                {
                    FullName = applicant?.FullName ?? "Unknown",
                    Position = vacancy?.PositionTitle ?? "Unknown",
                    Status = a.Status.ToString(),
                    SubmittedAt = a.SubmittedAt.ToString("MMM d, yyyy")
                });
            }

            TotalCount = $"{ReportItems.Count} record{(ReportItems.Count != 1 ? "s" : "")}";
        }
        catch (Exception ex)
        {
            Message = $"Error loading report: {ex.Message}";
            HasMessage = true;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand] private async Task FilterAll() { ReportTitle = "All applicants"; await LoadReportAsync(); }
    [RelayCommand] private async Task FilterSubmitted() { ReportTitle = "Submitted"; await LoadReportAsync(ApplicationStatus.Submitted); }
    [RelayCommand] private async Task FilterUnderReview() { ReportTitle = "Under review"; await LoadReportAsync(ApplicationStatus.UnderReview); }
    [RelayCommand] private async Task FilterShortlisted() { ReportTitle = "Shortlisted"; await LoadReportAsync(ApplicationStatus.Shortlisted); }
    [RelayCommand] private async Task FilterAccepted() { ReportTitle = "Accepted"; await LoadReportAsync(ApplicationStatus.Accepted); }
    [RelayCommand] private async Task FilterRejected() { ReportTitle = "Rejected"; await LoadReportAsync(ApplicationStatus.Rejected); }
    [RelayCommand] private async Task ApplyDateFilter() { await LoadReportAsync(_currentFilter); }

    [RelayCommand]
    private async Task ExportToText()
    {
        try
        {
            var sb = new StringBuilder();
            sb.AppendLine($"HR APPLICANT SYSTEM — {ReportTitle.ToUpper()}");
            sb.AppendLine($"Generated: {DateTime.Now:MMMM d, yyyy h:mm tt}");
            sb.AppendLine($"Total: {TotalCount}");
            sb.AppendLine(new string('-', 60));
            sb.AppendLine($"{"Name",-25} {"Position",-20} {"Status",-15} {"Submitted"}");
            sb.AppendLine(new string('-', 60));

            foreach (var item in ReportItems)
                sb.AppendLine($"{item.FullName,-25} {item.Position,-20} {item.Status,-15} {item.SubmittedAt}");

            var path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                $"Report_{DateTime.Now:yyyyMMdd_HHmmss}.txt");

            await File.WriteAllTextAsync(path, sb.ToString());

            Message = $"Exported to: {path}";
            HasMessage = true;
        }
        catch (Exception ex)
        {
            Message = $"Export failed: {ex.Message}";
            HasMessage = true;
        }
    }

    [RelayCommand]
    private async Task ExportToCsv()
    {
        try
        {
            var sb = new StringBuilder();
            sb.AppendLine("Full Name,Position,Status,Submitted At");

            foreach (var item in ReportItems)
                sb.AppendLine($"\"{item.FullName}\",\"{item.Position}\",\"{item.Status}\",\"{item.SubmittedAt}\"");

            var path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                $"Report_{DateTime.Now:yyyyMMdd_HHmmss}.csv");

            await File.WriteAllTextAsync(path, sb.ToString());

            Message = $"Exported to: {path}";
            HasMessage = true;
        }
        catch (Exception ex)
        {
            Message = $"Export failed: {ex.Message}";
            HasMessage = true;
        }
    }

    [RelayCommand] private void GoBack() => _mainViewModel?.NavigateToHRDashboard();
}