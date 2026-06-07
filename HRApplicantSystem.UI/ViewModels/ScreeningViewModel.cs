using System.Collections.ObjectModel;
using HRApplicantSystem.Data.Models;

namespace HRApplicantSystem.UI.ViewModels;

public class ScreeningViewModel : ViewModelBase
{
    // List of applications under review
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

    // Screening result — Qualified or Not Qualified
    private string _screeningResult = "Qualified";
    public string ScreeningResult
    {
        get => _screeningResult;
        set
        {
            _screeningResult = value;
            OnPropertyChanged();
        }
    }

    // Remarks from HR
    private string _remarks = "";
    public string Remarks
    {
        get => _remarks;
        set
        {
            _remarks = value;
            OnPropertyChanged();
        }
    }

    public ScreeningViewModel()
    {
        // Sample data for now — will connect to database later
        Applications.Add(new Application
        {
            Status = "Under Review"
        });
    }

    // Save Screening Result
    // Based on PDF: Qualified = Shortlisted, Not Qualified = Rejected
    public void SaveScreeningResult()
    {
        if (SelectedApplication == null) return;

        if (ScreeningResult == "Qualified")
        {
            SelectedApplication.Status = "Shortlisted";
        }
        else
        {
            SelectedApplication.Status = "Rejected";
        }

        OnPropertyChanged(nameof(SelectedApplication));
    }
}