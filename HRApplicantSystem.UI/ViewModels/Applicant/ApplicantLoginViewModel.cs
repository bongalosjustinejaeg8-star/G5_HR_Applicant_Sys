namespace HRApplicantSystem.UI.ViewModels.Applicant;

public partial class ApplicantLoginViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainViewModel;

    public ApplicantLoginViewModel(MainWindowViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
    }
}