using CommunityToolkit.Mvvm.ComponentModel;
using HRApplicantSystem.Data;
using HRApplicantSystem.Data.Repositories;
using HRApplicantSystem.Services.Implementations;
using HRApplicantSystem.UI.ViewModels.Applicant;
using HRApplicantSystem.UI.ViewModels.HR;

namespace HRApplicantSystem.UI.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private ViewModelBase _currentView;

    private static DbContext GetDb() => new DbContext(AppConfig.ConnectionString);

    private static ApplicationService GetAppService()
    {
        var db = GetDb();
        return new ApplicationService(
            new ApplicationRepository(db),
            new ApplicationStatusHistoryRepository(db),
            new JobVacancyRepository(db)
        );
    }

    private static ScreeningService GetScreeningService()
    {
        var db = GetDb();
        return new ScreeningService(
            new ApplicationRepository(db),
            new ApplicationStatusHistoryRepository(db),
            new ScreeningResultRepository(db)
        );
    }

    private static InterviewScheduleService GetInterviewScheduleService()
    {
        var db = GetDb();
        return new InterviewScheduleService(
            new ApplicationRepository(db),
            new InterviewScheduleRepository(db),
            new ApplicationStatusHistoryRepository(db)
        );
    }

    private static InterviewEvaluationService GetInterviewEvaluationService()
    {
        var db = GetDb();
        return new InterviewEvaluationService(
            new ApplicationRepository(db),
            new InterviewScheduleRepository(db),
            new InterviewEvaluationRepository(db),
            new ApplicationStatusHistoryRepository(db)
        );
    }

    private static JobVacancyService GetJobVacancyService()
    {
        var db = GetDb();
        return new JobVacancyService(new JobVacancyRepository(db));
    }

    public MainWindowViewModel()
    {
        _currentView = new LandingViewModel(this);
    }

    #region Authentication
    public void NavigateToApplicantLogin() => CurrentView = new ApplicantLoginViewModel(this);
    public void NavigateToHRLogin() => CurrentView = new HRLoginViewModel(this);
    public void NavigateToLanding() => CurrentView = new LandingViewModel(this);
    public void NavigateToRegister() => CurrentView = new RegisterViewModel(this);
    #endregion

    #region HR Navigation
    public void NavigateToHRDashboard() => CurrentView = new HRDashboardViewModel(this);
    public void NavigateToJobVacancyMgmt() => CurrentView = new JobVacancyMgmtViewModel();
    public void NavigateToApplicantList() => CurrentView = new ApplicantListViewModel();
    public void NavigateToApplicantReview() => CurrentView = new HRApplicantReviewViewModel(GetAppService());
    public void NavigateToScreening() => CurrentView = new ScreeningViewModel(GetScreeningService());
    public void NavigateToInterviewSchedule() => CurrentView = new InterviewScheduleViewModel(GetInterviewScheduleService());
    public void NavigateToInterviewEvaluation() => CurrentView = new InterviewEvaluationViewModel(GetInterviewEvaluationService());
    public void NavigateToHiringDecision() => CurrentView = new HiringDecisionViewModel();
    public void NavigateToReports() => CurrentView = new ReportsViewModel();
    public void NavigateToMaintenance() => CurrentView = new MaintenanceViewModel();
    #endregion

    #region Applicant Navigation
    public void NavigateToApplicantDashboard() => CurrentView = new DashboardViewModel(this);
    public void NavigateToProfile() => CurrentView = new ProfileViewModel();
    public void NavigateToJobVacancies() => CurrentView = new JobVacancyViewModel(GetJobVacancyService(), GetAppService());
    public void NavigateToMyApplication()=> CurrentView = new MyApplicationViewModel(GetAppService(), this); public void NavigateToMyDocuments() => CurrentView = new MyDocumentsViewModel();
    public void NavigateToStatusTracking() => CurrentView = new StatusTrackingViewModel();
    #endregion
}