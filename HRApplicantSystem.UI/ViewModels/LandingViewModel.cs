using System;
using System.Threading.Tasks;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;

namespace HRApplicantSystem.UI.ViewModels;

public partial class LandingViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainViewModel;

    public LandingViewModel(MainWindowViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
    }

    [RelayCommand]
    private void GoToApplicantLogin()
    {
        _mainViewModel.NavigateToApplicantLogin();
    }

    [RelayCommand]
    private void GoToHRLogin()
    {
        _mainViewModel.NavigateToHRLogin();
    }
}