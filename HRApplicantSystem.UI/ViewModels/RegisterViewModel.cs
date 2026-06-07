using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace HRApplicantSystem.UI.ViewModels
{
    public partial class RegisterViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel? _mainViewModel;

        [ObservableProperty] private string _regUsername = string.Empty;
        [ObservableProperty] private string _regPassword = string.Empty;
        [ObservableProperty] private string _confirmPassword = string.Empty;

        public RegisterViewModel()
        {
        }

        public RegisterViewModel(MainWindowViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }

        [RelayCommand]
        private void Register()
        {
            
            if (_mainViewModel != null)
            {
                _mainViewModel.NavigateTo(new LoginViewModel(_mainViewModel));
            }
        }

        [RelayCommand]
        private void BackToLogin()
        {
            if (_mainViewModel != null)
            {
                _mainViewModel.NavigateTo(new LoginViewModel(_mainViewModel));
            }
        }
    }
}