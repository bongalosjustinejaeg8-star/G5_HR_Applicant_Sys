using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRApplicantSystem.Data;
using HRApplicantSystem.Data.Repositories;
using HRApplicantSystem.Shared.Enums;
using HRApplicantSystem.Shared.Helpers;
using HRApplicantSystem.Services.Implementations;

namespace HRApplicantSystem.UI.ViewModels.HR;

public partial class HRLoginViewModel : ViewModelBase
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

    public HRLoginViewModel(MainWindowViewModel mainViewModel)
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

            var user = await authService.LoginHRAsync(Email, Password);

            if (user == null)
            {
                ErrorMessage = "Invalid email or password!";
                HasError = true;
                return;
            }

            var roleRepo = new RoleRepository(dbContext);
            var role = await roleRepo.GetByIdAsync(user.RoleId);
            var userRole = Enum.Parse<UserRole>(role?.RoleName ?? "HRStaff");

            SessionManager.Login(user.Email, user.UserId, userRole);
            _mainViewModel.NavigateToHRDashboard();
        }
        catch (Exception ex)
        {
            Console.WriteLine("=== HR LOGIN ERROR ===");
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            ErrorMessage = ex.Message;
            HasError = true;
        }
    }
    [RelayCommand] private void GoBack() => _mainViewModel.NavigateToLanding();
}
