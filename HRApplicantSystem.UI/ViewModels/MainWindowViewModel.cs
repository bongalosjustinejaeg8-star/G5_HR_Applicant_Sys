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
}