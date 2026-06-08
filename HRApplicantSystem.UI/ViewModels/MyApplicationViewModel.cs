using System.Collections.ObjectModel;
using HRApplicantSystem.Data.Models;
using HRApplicantSystem.Services.Interfaces;
using HRApplicantSystem.Shared.Enums;
using HRApplicantSystem.Shared.Helpers;

namespace HRApplicantSystem.UI.ViewModels;

public class MyApplicationViewModel : ViewModelBase
{
    private readonly IApplicationService _applicationService;

    public ObservableCollection<Application> Applications { get; set; } = new();

    private Application? _selectedApplication;
    public Application? SelectedApplication
    {
        get => _selectedApplication;
        set
        {
            _selectedApplication = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsEditable));
            OnPropertyChanged(nameof(IsLocked));
        }
    }

    private string _message = "";
    public string Message
    {
        get => _message;
        set { _message = value; OnPropertyChanged(); }
    }

    // Based on PDF: only Draft and Submitted can be edited
    public bool IsEditable =>
        SelectedApplication?.Status == ApplicationStatus.Draft ||
        SelectedApplication?.Status == ApplicationStatus.Submitted;

    // Based on PDF: locked when HR starts review
    public bool IsLocked =>
        SelectedApplication != null && !IsEditable;

    public MyApplicationViewModel(IApplicationService applicationService)
    {
        _applicationService = applicationService;
    }

    public async Task LoadApplicationsAsync()
    {
        var applicantId = SessionManager.CurrentApplicantId;
        var apps = await _applicationService.GetByApplicantIdAsync(applicantId!);
        Applications.Clear();
        foreach (var app in apps)
            Applications.Add(app);
    }

    public async Task SubmitApplicationAsync()
    {
        if (SelectedApplication == null) return;
        var applicantId = SessionManager.CurrentApplicantId;
        Message = await _applicationService.SubmitApplicationAsync(
            SelectedApplication.ApplicationId, applicantId!);
        await LoadApplicationsAsync();
    }
}