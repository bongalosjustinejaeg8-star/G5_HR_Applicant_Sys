using System;
using System.Collections.ObjectModel;
using HRApplicantSystem.Data.Models;

namespace HRApplicantSystem.UI.ViewModels;

public class InterviewScheduleViewModel : ViewModelBase
{
    // List of shortlisted applicants
    public ObservableCollection<Application> Applications { get; set; } = new();

    // Selected application
    private Application? _selectedApplication;
    public Application? SelectedApplication
    {
        get => _selectedApplication;
        set
        {
            _selectedApplication = value;
            OnPropertyChanged();
        }
    }

    // Interview Date
    private DateTime _interviewDate = DateTime.Now;
    public DateTime InterviewDate
    {
        get => _interviewDate;
        set
        {
            _interviewDate = value;
            OnPropertyChanged();
        }
    }

    // Interviewer Name
    private string _interviewer = "";
    public string Interviewer
    {
        get => _interviewer;
        set
        {
            _interviewer = value;
            OnPropertyChanged();
        }
    }

    // Mode — Online or On-site
    private string _mode = "Online";
    public string Mode
    {
        get => _mode;
        set
        {
            _mode = value;
            OnPropertyChanged();
        }
    }

    // Location
    private string _location = "";
    public string Location
    {
        get => _location;
        set
        {
            _location = value;
            OnPropertyChanged();
        }
    }

    public InterviewScheduleViewModel()
    {
        // Sample data for now — will connect to database later
        Applications.Add(new Application
        {
            Status = "Shortlisted"
        });
    }

    // Save Interview Schedule
    // Based on PDF: blocks past dates, saves to InterviewSchedules table
    public bool SaveSchedule()
    {
        if (SelectedApplication == null) return false;

        // Based on PDF: reject dates in the past
        if (InterviewDate < DateTime.Now)
        {
            return false;
        }

        // Update application status to For Interview
        SelectedApplication.Status = "For Interview";
        OnPropertyChanged(nameof(SelectedApplication));

        return true;
    }
}