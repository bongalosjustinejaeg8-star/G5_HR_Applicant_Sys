using System.Collections.ObjectModel;
using System.Threading.Tasks;
using HRApplicantSystem.Data.Models;
using HRApplicantSystem.Services.Interfaces;
using HRApplicantSystem.Shared.Enums;
using HRApplicantSystem.Shared.Helpers;
using CommunityToolkit.Mvvm.Input;

namespace HRApplicantSystem.UI.ViewModels.Applicant;

public partial class MyApplicationViewModel : ViewModelBase
{
    private readonly IApplicationService _applicationService;
    private readonly MainWindowViewModel? _mainViewModel;
    public ObservableCollection<Application> Applications { get; } = new();

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
        set
        {
            _message = value;
            OnPropertyChanged();
        }
    }

    public bool IsEditable =>
        SelectedApplication?.Status == ApplicationStatus.Draft ||
        SelectedApplication?.Status == ApplicationStatus.Submitted;

    public bool IsLocked =>
        SelectedApplication != null && !IsEditable;

    public MyApplicationViewModel(
    IApplicationService applicationService,
    MainWindowViewModel mainViewModel)
    {
        _applicationService = applicationService;
        _mainViewModel = mainViewModel;
    }

    public async Task LoadApplicationsAsync()
    {
        var applicantId = SessionManager.CurrentUserId;

        var apps = await _applicationService.GetByApplicantIdAsync(applicantId!);

        Applications.Clear();

        foreach (var app in apps)
            Applications.Add(app);
    }

    public async Task SubmitApplicationAsync()
    {
        if (SelectedApplication == null)
            return;

        var applicantId = SessionManager.CurrentUserId;

        bool success =
            await _applicationService.SubmitApplicationAsync(
                SelectedApplication.ApplicationId,
                applicantId!);

        Message = success
            ? "Application submitted!"
            : "Something went wrong.";

        await LoadApplicationsAsync();


    }
    [RelayCommand] public void BackDash() => _mainViewModel?.NavigateToApplicantDashboard();

}