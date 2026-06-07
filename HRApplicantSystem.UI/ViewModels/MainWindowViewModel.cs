using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRApplicantSystem.UI.ViewModels;

namespace HRApplicantSystem.UI.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        [ObservableProperty] private ViewModelBase _currentPage = new LoginViewModel();

        [RelayCommand] private void NavigateToDashboard() => CurrentPage = new DashboardViewModel();
        [RelayCommand] private void NavigateToProfile() => CurrentPage = new ProfileViewModel();
        [RelayCommand] private void NavigateToDocuments() => CurrentPage = new DocumentsViewModel();
        [RelayCommand] private void NavigateToStatusTracking() => CurrentPage = new StatusTrackingViewModel();
        [RelayCommand] private void NavigateToRegister() => CurrentPage = new RegisterViewModel();
        [RelayCommand] private void NavigateToLogin() => CurrentPage = new LoginViewModel();
    }
}