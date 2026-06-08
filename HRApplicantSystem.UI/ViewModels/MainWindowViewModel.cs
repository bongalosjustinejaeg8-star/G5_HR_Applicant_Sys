using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRApplicantSystem.UI.ViewModels.Applicant;
using HRApplicantSystem.UI.ViewModels.HR;

namespace HRApplicantSystem.UI.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    // currently displayed view
    [ObservableProperty]
    private ViewModelBase _currentView;

    public MainWindowViewModel()
    {
        // start with landing view
        _currentView = new LandingViewModel(this);
    }

    #region Authentication Navigation
    // called when "Applicant" button is clicked
    public void NavigateToApplicantLogin()
    {
        CurrentView = new ApplicantLoginViewModel(this);
    }

    // called when "HR / Staff" button is clicked
    public void NavigateToHRLogin()
    {
        CurrentView = new HRLoginViewModel(this);
    }

    public void NavigateToLanding()
    {
        CurrentView = new LandingViewModel(this);
    }

    public void NavigateToRegister()
    {
        CurrentView = new RegisterViewModel(this);
    }
    #endregion

    #region HR Navigation
    public void NavigateToHRDashboard()
    {
        // TODO: Implement when HRDashboardViewModel is created
        // CurrentView = new HRDashboardViewModel(this);
    }

    public void NavigateToJobVacancyMgmt()
    {
        // TODO: Implement when JobVacancyMgmtViewModel is created
        // CurrentView = new JobVacancyMgmtViewModel(this);
    }

    public void NavigateToApplicantList()
    {
        // TODO: Implement when ApplicantListViewModel is created
        // CurrentView = new ApplicantListViewModel(this);
    }

    public void NavigateToApplicantReview()
    {
        // TODO: Implement when ApplicantReviewViewModel is created
        // CurrentView = new HRApplicantReviewViewModel(this);
    }

    public void NavigateToScreening()
    {
        // TODO: Implement when ScreeningViewModel is created
        // CurrentView = new ScreeningViewModel(this);
    }

    public void NavigateToInterviewSchedule()
    {
        // TODO: Implement when InterviewScheduleViewModel is created
        // CurrentView = new InterviewScheduleViewModel(this);
    }

    public void NavigateToInterviewEvaluation()
    {
        // TODO: Implement when InterviewEvaluationViewModel is created
        // CurrentView = new InterviewEvaluationViewModel(this);
    }

    public void NavigateToHiringDecision()
    {
        // TODO: Implement when HiringDecisionViewModel is created
        // CurrentView = new HiringDecisionViewModel(this);
    }

    public void NavigateToReports()
    {
        // TODO: Implement when ReportsViewModel is created
        // CurrentView = new ReportsViewModel(this);
    }

    public void NavigateToMaintenance()
    {
        // TODO: Implement when MaintenanceViewModel is created
        // CurrentView = new MaintenanceViewModel(this);
    }
    #endregion

    #region Applicant Navigation
    public void NavigateToApplicantDashboard()
    {
        // TODO: Implement when ApplicantDashboardViewModel is created
        // CurrentView = new ApplicantDashboardViewModel(this);
    }

    public void NavigateToProfile()
    {
        // TODO: Implement when ProfileViewModel is created
        // CurrentView = new ProfileViewModel(this);
    }

    public void NavigateToJobVacancies()
    {
        // TODO: Implement when JobVacanciesViewModel is created
        // CurrentView = new JobVacanciesViewModel(this);
    }

    public void NavigateToMyApplications()
    {
        // TODO: Implement when MyApplicationsViewModel is created
        // CurrentView = new MyApplicationsViewModel(this);
    }

    public void NavigateToMyDocuments()
    {
        // TODO: Implement when MyDocumentsViewModel is created
        // CurrentView = new MyDocumentsViewModel(this);
    }

    public void NavigateToStatusTracking()
    {
        // TODO: Implement when StatusTrackingViewModel is created
        // CurrentView = new StatusTrackingViewModel(this);
    }
    #endregion
}
