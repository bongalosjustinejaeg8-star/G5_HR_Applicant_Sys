using HRApplicantSystem.Shared.Helpers;

namespace HRApplicantSystem.UI.ViewModels;

public class ReportsViewModel : ViewModelBase
{
    private string? _selectedReportType;
    public string? SelectedReportType
    {
        get => _selectedReportType;
        set { _selectedReportType = value; OnPropertyChanged(); }
    }

    private DateTime _startDate = DateTime.Now.AddMonths(-1);
    public DateTime StartDate
    {
        get => _startDate;
        set { _startDate = value; OnPropertyChanged(); }
    }

    private DateTime _endDate = DateTime.Now;
    public DateTime EndDate
    {
        get => _endDate;
        set { _endDate = value; OnPropertyChanged(); }
    }

    private string _reportData = "";
    public string ReportData
    {
        get => _reportData;
        set { _reportData = value; OnPropertyChanged(); }
    }

    public ReportsViewModel()
    {
        // TODO: Initialize reports
    }

    public async Task GenerateReportAsync()
    {
        // TODO: Implement report generation
    }

    public async Task ExportToPdfAsync()
    {
        // TODO: Implement PDF export
    }
}
