using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using HRApplicantSystem.Data.Models;
using HRApplicantSystem.Services.Interfaces;
using HRApplicantSystem.Shared.Helpers;

namespace HRApplicantSystem.UI.ViewModels;

public class InterviewEvaluationViewModel : ViewModelBase
{
    private readonly IInterviewEvaluationService _evaluationService;

    public ObservableCollection<Application> Applications { get; set; } = new();

    private Application? _selectedApplication;
    public Application? SelectedApplication
    {
        get => _selectedApplication;
        set { _selectedApplication = value; OnPropertyChanged(); }
    }

    private int _score;
    public int Score
    {
        get => _score;
        set { _score = value; OnPropertyChanged(); }
    }

    private string _remarks = "";
    public string Remarks
    {
        get => _remarks;
        set { _remarks = value; OnPropertyChanged(); }
    }

    private string _passFail = "Pass";
    public string PassFail
    {
        get => _passFail;
        set { _passFail = value; OnPropertyChanged(); }
    }

    private string _recommendation = "";
    public string Recommendation
    {
        get => _recommendation;
        set { _recommendation = value; OnPropertyChanged(); }
    }

    private string _message = "";
    public string Message
    {
        get => _message;
        set { _message = value; OnPropertyChanged(); }
    }

    public InterviewEvaluationViewModel(IInterviewEvaluationService evaluationService)
    {
        _evaluationService = evaluationService;
    }

    public async Task LoadApplicationsAsync()
    {
        var apps = await _evaluationService.GetApplicationsForEvaluationAsync();
        Applications.Clear();
        foreach (var app in apps)
            Applications.Add(app);
    }

    public async Task SaveEvaluationAsync()
    {
        if (SelectedApplication == null) return;
        var hrUserId = SessionManager.CurrentUserId;
        Message = await _evaluationService.SaveEvaluationAsync(
            SelectedApplication.ApplicationId, hrUserId!,
            Score, Remarks, PassFail, Recommendation);
        await LoadApplicationsAsync();
    }
}
