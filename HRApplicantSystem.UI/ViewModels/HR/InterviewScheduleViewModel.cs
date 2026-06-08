using System;
using System.Collections.ObjectModel;
using HRApplicantSystem.Data.Models;
using HRApplicantSystem.Services.Interfaces;
using HRApplicantSystem.Shared.Enums;
using HRApplicantSystem.Shared.Helpers;

namespace HRApplicantSystem.UI.ViewModels;

public class InterviewScheduleViewModel : ViewModelBase
{
    private readonly IInterviewScheduleService _scheduleService;

    public ObservableCollection<Application> Applications { get; set; } = new();

    private Application? _selectedApplication;
    public Application? SelectedApplication
    {
        get => _selectedApplication;
        set { _selectedApplication = value; OnPropertyChanged(); }
    }

    private DateTime _interviewDate = DateTime.Now;
    public DateTime InterviewDate
    {
        get => _interviewDate;
        set { _interviewDate = value; OnPropertyChanged(); }
    }

    private string _interviewer = "";
    public string Interviewer
    {
        get => _interviewer;
        set { _interviewer = value; OnPropertyChanged(); }
    }

    private InterviewMode _mode = InterviewMode.Online;
    public InterviewMode Mode
    {
        get => _mode;
        set { _mode = value; OnPropertyChanged(); }
    }

    private string _location = "";
    public string Location
    {
        get => _location;
        set { _location = value; OnPropertyChanged(); }
    }

    private string _message = "";
    public string Message
    {
        get => _message;
        set { _message = value; OnPropertyChanged(); }
    }

    public InterviewScheduleViewModel(IInterviewScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    public async Task LoadApplicationsAsync()
    {
        var apps = await _scheduleService.GetShortlistedApplicationsAsync();
        Applications.Clear();
        foreach (var app in apps)
            Applications.Add(app);
    }

    public async Task SaveScheduleAsync()
    {
        if (SelectedApplication == null) return;
        var hrUserId = SessionManager.CurrentUserId;
        Message = await _scheduleService.ScheduleInterviewAsync(
            SelectedApplication.ApplicationId, hrUserId!,
            InterviewDate, Interviewer, Mode, Location);
        await LoadApplicationsAsync();
    }
}
