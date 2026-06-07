using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace HRApplicantSystem.UI.ViewModels
{
    public partial class LoginViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel? _mainViewModel;

        [ObservableProperty] private string _username = string.Empty;
        [ObservableProperty] private string _password = string.Empty;

        public LoginViewModel()
        {
        }

        public LoginViewModel(MainWindowViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }

        [RelayCommand]
        private void Login()
        {
            if (_mainViewModel != null)
            {
                _mainViewModel.NavigateTo(new DashboardViewModel(_mainViewModel));
            }
        }

        [RelayCommand]
        private void NavigateToRegister()
        {
            if (_mainViewModel != null)
            {
                _mainViewModel.NavigateTo(new RegisterViewModel(_mainViewModel));
            }
        }
    }
}