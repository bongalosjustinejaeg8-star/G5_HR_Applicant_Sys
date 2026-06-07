using System.Collections.ObjectModel;
using HRApplicantSystem.Data.Models;

namespace HRApplicantSystem.UI.ViewModels;

public class InterviewEvaluationViewModel : ViewModelBase
{
    // List of applications for interview
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

    // Score 1-100
    private int _score;
    public int Score
    {
        get => _score;
        set
        {
            _score = value;
            OnPropertyChanged();
        }
    }

    // Remarks
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

    // Pass or Fail
    private string _passFail = "Pass";
    public string PassFail
    {
        get => _passFail;
        set
        {
            _passFail = value;
            OnPropertyChanged();
        }
    }

    // Recommendation
    private string _recommendation = "";
    public string Recommendation
    {
        get => _recommendation;
        set
        {
            _recommendation = value;
            OnPropertyChanged();
        }
    }

    public InterviewEvaluationViewModel()
    {
        // Sample data for now — will connect to database later
        Applications.Add(new Application
        {
            Status = "For Interview"
        });
    }

    // Save Evaluation
    // Based on PDF: Pass = For Final Review, Fail = Rejected
    public void SaveEvaluation()
    {
        if (SelectedApplication == null) return;

        // Validate score
        if (Score < 1 || Score > 100) return;

        if (PassFail == "Pass")
        {
            // Based on PDF: Pass moves to For Final Review
            SelectedApplication.Status = "For Final Review";
        }
        else
        {
            // Based on PDF: Fail moves to Rejected
            SelectedApplication.Status = "Rejected";
        }

        OnPropertyChanged(nameof(SelectedApplication));
    }
}