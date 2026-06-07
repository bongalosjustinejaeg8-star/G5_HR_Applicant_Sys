using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using HRApplicantSystem.UI.ViewModels;

namespace HRApplicantSystem.UI.ViewModels
{
    public partial class RegisterViewModel : ViewModelBase
    {
        [ObservableProperty] private string _fullName = string.Empty;
        [ObservableProperty] private string _email = string.Empty;
        [ObservableProperty] private string _password = string.Empty;
        [ObservableProperty] private string _confirmPassword = string.Empty;

        [RelayCommand]
        private async Task Register()
        {
            
            await Task.Delay(100);
        } 
    }
}