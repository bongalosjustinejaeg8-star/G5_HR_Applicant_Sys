using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRApplicantSystem.Data;
using HRApplicantSystem.Data.Repositories;
using HRApplicantSystem.Shared.Helpers;
using HRApplicantSystem.Services.Implementations;

namespace HRApplicantSystem.UI.ViewModels.Applicant;

public partial class RegisterViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainViewModel;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _confirmPassword = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _hasError = false;

    public RegisterViewModel(MainWindowViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
    }

    [RelayCommand]
    private async Task RegisterAsync()
    {
        HasError = false;

        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Please enter your email and password.";
            HasError = true;
            return;
        }

        if (Password != ConfirmPassword)
        {
            ErrorMessage = "Passwords do not match!";
            HasError = true;
            return;
        }

        try
        {
            var dbContext = new DbContext(AppConfig.ConnectionString);
            var authService = new AuthService(
                new ApplicantAccountRepository(dbContext),
                new UserRepository(dbContext),
                new PasswordHasher()
            );

            bool success = await authService.RegisterApplicantAsync(Email, Password);

            if (!success)
            {
                ErrorMessage = "Email already exists!";
                HasError = true;
                return;
            }

            // navigate to login after successful registration
            _mainViewModel.NavigateToApplicantLogin();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            HasError = true;
        }
    }

    [RelayCommand]
    private void GoToLogin()
    {
        _mainViewModel.NavigateToApplicantLogin();
    }

    [RelayCommand]
    private void GoBack()
    {
        _mainViewModel.NavigateToLanding();
    }
}