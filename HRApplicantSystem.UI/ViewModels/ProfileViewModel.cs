using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRApplicantSystem.UI.ViewModels;

namespace HRApplicantSystem.UI.ViewModels
{
    public partial class ProfileViewModel : ViewModelBase
    {
        [ObservableProperty] private string _adminName = "Aliyah Lopez";
        [ObservableProperty] private string _email = "aliyahlopez@gmail.com";
        [ObservableProperty] private string _role = "HR Administrator";
        [ObservableProperty] private string _department = "Human Resources Department";
        [ObservableProperty] private bool _isEditMode = false;
        [ObservableProperty] private string _buttonText = "📝 Edit Profile";

        [RelayCommand]
        private void ToggleEdit()
        {
            IsEditMode = !IsEditMode;
            ButtonText = IsEditMode ? "💾 Save Changes" : "📝 Edit Profile";
        }
    }
}