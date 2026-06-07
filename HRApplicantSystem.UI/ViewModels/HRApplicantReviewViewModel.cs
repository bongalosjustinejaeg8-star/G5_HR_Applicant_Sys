using System.Collections.ObjectModel;
using HRApplicantSystem.Data.Models;

namespace HRApplicantSystem.UI.ViewModels;

public class HRApplicantReviewViewModel : ViewModelBase
{
    // List of submitted applications for HR to review
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

    // Search text
    private string _searchText = "";
    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            OnPropertyChanged();
        }
    }

    public HRApplicantReviewViewModel()
    {
        // Sample data for now — will connect to database later
        Applications.Add(new Application
        {
            Status = "Submitted"
        });

        Applications.Add(new Application
        {
            Status = "Submitted"
        });
    }

    // Start Review method
    // Based on PDF: changes status to Under Review and locks application
    public void StartReview()
    {
        if (SelectedApplication == null) return;

        if (SelectedApplication.Status != "Submitted")
        {
            return;
        }

        // Lock the application and change status
        SelectedApplication.Status = "Under Review";
        SelectedApplication.IsLocked = true;

        OnPropertyChanged(nameof(SelectedApplication));
    }
}