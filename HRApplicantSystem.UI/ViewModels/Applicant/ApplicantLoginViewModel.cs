using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRApplicantSystem.UI.ViewModels;

namespace HRApplicantSystem.UI.ViewModels.Applicant;

public partial class ApplicantLoginViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainViewModel;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _hasError = false;

    public ApplicantLoginViewModel(MainWindowViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        // will implement auth logic later
        HasError = false;

        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Please enter your email and password.";
            HasError = true;
            return;
        }

        // TODO: call AuthService here
    }

    [RelayCommand]
    private void GoToRegister()
    {
        _mainViewModel.NavigateToRegister();
    }

    [RelayCommand]
    private void GoBack()
    {
        _mainViewModel.NavigateToLanding();
    }
}