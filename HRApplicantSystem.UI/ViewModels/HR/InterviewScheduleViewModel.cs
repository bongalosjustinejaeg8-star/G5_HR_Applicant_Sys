using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRApplicantSystem.Data.Models;
using HRApplicantSystem.Services.Interfaces;
using HRApplicantSystem.Shared.Enums;
using HRApplicantSystem.Shared.Helpers;

namespace HRApplicantSystem.UI.ViewModels.HR;

/// <summary>
/// ViewModel for Interview Scheduling module.
/// Handles scheduling interviews for shortlisted applicants.
/// </summary>
public partial class InterviewScheduleViewModel : ViewModelBase
{
    private readonly IInterviewScheduleService _scheduleService;
    private readonly MainWindowViewModel? _mainViewModel;

    /// <summary>
    /// Collection of shortlisted applications available for scheduling.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<Application> applications = new();

    /// <summary>
    /// Currently selected application for scheduling.
    /// </summary>
    [ObservableProperty]
    private Application? selectedApplication;

    /// <summary>
    /// Date for the interview.
    /// </summary>
    [ObservableProperty]
    private DateTime interviewDate = DateTime.Now.AddDays(7);

    /// <summary>
    /// Name of the interviewer.
    /// </summary>
    [ObservableProperty]
    private string interviewer = "";

    /// <summary>
    /// Mode of the interview (Online or Onsite).
    /// </summary>
    [ObservableProperty]
    private InterviewMode mode = InterviewMode.Online;

    /// <summary>
    /// Location for onsite interviews.
    /// </summary>
    [ObservableProperty]
    private string location = "";

    /// <summary>
    /// Status or error message.
    /// </summary>
    [ObservableProperty]
    private string message = "";

    /// <summary>
    /// True while operations are in progress.
    /// </summary>
    [ObservableProperty]
    private bool isLoading = false;

    public InterviewScheduleViewModel(IInterviewScheduleService scheduleService)
    {
        Debug.WriteLine("[InterviewScheduleViewModel] Initializing with service");
        _scheduleService = scheduleService;
        _ = InitializeAsync();
    }

    public InterviewScheduleViewModel(IInterviewScheduleService scheduleService, MainWindowViewModel mainViewModel)
    {
        Debug.WriteLine("[InterviewScheduleViewModel] Initializing with service and MainWindowViewModel");
        _scheduleService = scheduleService;
        _mainViewModel = mainViewModel;
        _ = InitializeAsync();
    }

    /// <summary>
    /// Initializes the ViewModel and loads applications.
    /// </summary>
    private async Task InitializeAsync()
    {
        Debug.WriteLine("[InterviewScheduleViewModel] InitializeAsync called");
        try
        {
            await LoadApplicationsAsync();
            Message = "Interview scheduling ready.";
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[InterviewScheduleViewModel] InitializeAsync error: {ex.Message}");
            Message = "Failed to initialize interview scheduling.";
        }
    }

    /// <summary>
    /// Loads all shortlisted applications ready for interview scheduling.
    /// </summary>
    [RelayCommand]
    public async Task LoadApplicationsAsync()
    {
        Debug.WriteLine("[InterviewScheduleViewModel] LoadApplicationsAsync called");
        try
        {
            IsLoading = true;
            Applications.Clear();

            var apps = await _scheduleService.GetShortlistedApplicationsAsync();
            foreach (var app in apps)
            {
                Applications.Add(app);
            }

            Message = $"Loaded {Applications.Count} applications ready for scheduling.";
            Debug.WriteLine($"[InterviewScheduleViewModel] Loaded {Applications.Count} applications");
        }
        catch (Exception ex)
        {
            Message = $"Error loading applications: {ex.Message}";
            Debug.WriteLine($"[InterviewScheduleViewModel] LoadApplicationsAsync error: {ex}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Saves the interview schedule for the selected application.
    /// </summary>
    [RelayCommand]
    public async Task SaveScheduleAsync()
    {
        Debug.WriteLine("[InterviewScheduleViewModel] SaveScheduleAsync called");
        try
        {
            if (SelectedApplication == null)
            {
                Message = "Please select an application.";
                return;
            }

            if (string.IsNullOrWhiteSpace(Interviewer))
            {
                Message = "Please enter the interviewer name.";
                return;
            }

            if (InterviewDate < DateTime.Now)
            {
                Message = "Interview date must be in the future.";
                return;
            }

            if (Mode == InterviewMode.Onsite && string.IsNullOrWhiteSpace(Location))
            {
                Message = "Please enter the location for onsite interviews.";
                return;
            }

            IsLoading = true;
            var hrUserId = SessionManager.CurrentUserId;
            
            Message = await _scheduleService.ScheduleInterviewAsync(
                SelectedApplication.ApplicationId, 
                hrUserId!,
                InterviewDate, 
                Interviewer, 
                Mode, 
                Location);

            Debug.WriteLine($"[InterviewScheduleViewModel] Interview scheduled: {Message}");
            
            // Reset form
            Interviewer = "";
            Location = "";
            InterviewDate = DateTime.Now.AddDays(7);
            Mode = InterviewMode.Online;
            SelectedApplication = null;

            // Reload applications
            await LoadApplicationsAsync();
        }
        catch (Exception ex)
        {
            Message = $"Error saving schedule: {ex.Message}";
            Debug.WriteLine($"[InterviewScheduleViewModel] SaveScheduleAsync error: {ex}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Navigates back to the HR dashboard.
    /// </summary>
    [RelayCommand]
    public void GoBack()
    {
        Debug.WriteLine("[InterviewScheduleViewModel] GoBack called");
        _mainViewModel?.NavigateToHRDashboard();
    }
}
