using System.Collections.ObjectModel;
using HRApplicantSystem.Data.Models;
using HRApplicantSystem.Services.Interfaces;
using HRApplicantSystem.Shared.Helpers;

namespace HRApplicantSystem.UI.ViewModels;

public class HRApplicantReviewViewModel : ViewModelBase
{
    private readonly IApplicationService _applicationService;

    public ObservableCollection<Application> Applications { get; set; } = new();

    private Application? _selectedApplication;
    public Application? SelectedApplication
    {
        get => _selectedApplication;
        set { _selectedApplication = value; OnPropertyChanged(); }
    }

    private string _searchText = "";
    public string SearchText
    {
        get => _searchText;
        set { _searchText = value; OnPropertyChanged(); }
    }

    private string _message = "";
    public string Message
    {
        get => _message;
        set { _message = value; OnPropertyChanged(); }
    }

    public HRApplicantReviewViewModel(IApplicationService applicationService)
    {
        _applicationService = applicationService;
    }

    public async Task LoadApplicationsAsync()
    {
        var apps = await _applicationService.GetAllSubmittedAsync();
        Applications.Clear();
        foreach (var app in apps)
            Applications.Add(app);
    }

    public async Task StartReviewAsync()
    {
        if (SelectedApplication == null) return;
        var hrUserId = SessionManager.CurrentUserId;
        Message = await _applicationService.StartReviewAsync(
            SelectedApplication.ApplicationId, hrUserId!);
        await LoadApplicationsAsync();
    }
}