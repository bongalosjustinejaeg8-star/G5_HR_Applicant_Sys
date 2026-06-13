using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using HRApplicantSystem.Data.Models;
using HRApplicantSystem.Services.Interfaces;
using HRApplicantSystem.Shared.Helpers;

namespace HRApplicantSystem.UI.ViewModels;

public class ApplicantReviewViewModel : ViewModelBase
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

    public ApplicantReviewViewModel(IApplicationService applicationService)
    {
        _applicationService = applicationService;
    }

    public async Task LoadApplicationsAsync()
    {
        var apps = await _applicationService.GetAllAsync();
        Applications.Clear();
        foreach (var app in apps)
            Applications.Add(app);
    }

    public async Task StartReviewAsync()
    {
        if (SelectedApplication == null) return;
        var hrUserId = SessionManager.CurrentUserId;
        bool success = await _applicationService.StartReviewAsync(
            SelectedApplication.ApplicationId, hrUserId!);
        Message = success ? "Review started successfully!" : "Something went wrong.";
        await LoadApplicationsAsync();
    }
}
