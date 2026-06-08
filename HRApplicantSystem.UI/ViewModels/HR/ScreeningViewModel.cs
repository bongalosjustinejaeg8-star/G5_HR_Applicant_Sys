using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using HRApplicantSystem.Data.Models;
using HRApplicantSystem.Services.Interfaces;
using HRApplicantSystem.Shared.Enums;
using HRApplicantSystem.Shared.Helpers;

namespace HRApplicantSystem.UI.ViewModels;

public class ScreeningViewModel : ViewModelBase
{
    private readonly IScreeningService _screeningService;

    public ObservableCollection<Application> Applications { get; set; } = new();

    private Application? _selectedApplication;
    public Application? SelectedApplication
    {
        get => _selectedApplication;
        set { _selectedApplication = value; OnPropertyChanged(); }
    }

    private ScreeningResults _screeningResult = ScreeningResults.Qualified;
    public ScreeningResults ScreeningResult
    {
        get => _screeningResult;
        set { _screeningResult = value; OnPropertyChanged(); }
    }

    private string _remarks = "";
    public string Remarks
    {
        get => _remarks;
        set { _remarks = value; OnPropertyChanged(); }
    }

    private string _message = "";
    public string Message
    {
        get => _message;
        set { _message = value; OnPropertyChanged(); }
    }

    public ScreeningViewModel(IScreeningService screeningService)
    {
        _screeningService = screeningService;
    }

    public async Task LoadApplicationsAsync()
    {
        var apps = await _screeningService.GetApplicationsForScreeningAsync();
        Applications.Clear();
        foreach (var app in apps)
            Applications.Add(app);
    }

    public async Task SaveScreeningResultAsync()
    {
        if (SelectedApplication == null) return;
        var hrUserId = SessionManager.CurrentUserId;
        Message = await _screeningService.SaveScreeningResultAsync(
            SelectedApplication.ApplicationId, hrUserId!, ScreeningResult, Remarks);
        await LoadApplicationsAsync();
    }
}
