using System.Collections.ObjectModel;
using HRApplicantSystem.Shared.Helpers;

namespace HRApplicantSystem.UI.ViewModels;

public class HiringDecisionViewModel : ViewModelBase
{
    public ObservableCollection<dynamic> ApplicantsForDecision { get; set; } = new();

    private dynamic? _selectedApplicant;
    public dynamic? SelectedApplicant
    {
        get => _selectedApplicant;
        set { _selectedApplicant = value; OnPropertyChanged(); }
    }

    private string _decisionRemarks = "";
    public string DecisionRemarks
    {
        get => _decisionRemarks;
        set { _decisionRemarks = value; OnPropertyChanged(); }
    }

    private string _message = "";
    public string Message
    {
        get => _message;
        set { _message = value; OnPropertyChanged(); }
    }

    public HiringDecisionViewModel()
    {
        // TODO: Initialize hiring decision view
    }

    public async Task LoadApplicantsForDecisionAsync()
    {
        // TODO: Load applicants ready for decision
    }

    public async Task SubmitDecisionAsync()
    {
        // TODO: Implement decision submission
    }
}
