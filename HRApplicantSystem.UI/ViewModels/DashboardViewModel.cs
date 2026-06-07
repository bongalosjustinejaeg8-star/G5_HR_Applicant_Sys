using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace HRApplicantSystem.UI.ViewModels
{
    public partial class DashboardViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel _mainViewModel;

        [ObservableProperty]
        private ViewModelBase _currentSubPage;

        [ObservableProperty] private string _applicantName = "Juan dela Cruz";
        [ObservableProperty] private string _applicantInitials = "JC";

        public DashboardViewModel(MainWindowViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            _currentSubPage = new ProfileViewModel(); 
        }

        [RelayCommand]
        private void NavigateToProfile()
        {
            CurrentSubPage = new ProfileViewModel();
        }

        [RelayCommand]
        private void NavigateToDocuments()
        {
            CurrentSubPage = new DocumentsViewModel();
        }

        [RelayCommand]
        private void NavigateToStatus()
        {
            CurrentSubPage = new StatusTrackingViewModel();
        }

        [RelayCommand]
        private void Logout()
        {
            _mainViewModel.NavigateTo(new LoginViewModel(_mainViewModel));
        }
    }
}