using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRApplicantSystem.Data;
using HRApplicantSystem.Data.Repositories;
using HRApplicantSystem.Shared.Enums;
using HRApplicantSystem.Shared.Helpers;
using HRApplicantSystem.Services.Implementations;

namespace HRApplicantSystem.UI.ViewModels.Applicant;

public partial class ApplicantLoginViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainViewModel;

    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private string _password = string.Empty;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private bool _hasError = false;

    public ApplicantLoginViewModel(MainWindowViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        HasError = false;
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Please enter your email and password.";
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
            var account = await authService.LoginApplicantAsync(Email, Password);
            if (account == null)
            {
                ErrorMessage = "Invalid email or password!";
                HasError = true;
                var hasher = new PasswordHasher();
                var hash = hasher.Hash("hr123");
                Console.WriteLine(hash);
                return;
            }
            SessionManager.Login(account.Email, account.AccountId, UserRole.None);
            _mainViewModel.NavigateToApplicantDashboard();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            HasError = true;
        }
    }

    [RelayCommand] private void GoToRegister() => _mainViewModel.NavigateToRegister();
    [RelayCommand] private void GoBack() => _mainViewModel.NavigateToLanding();
}