using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace HRApplicantSystem.UI.ViewModels
{
    public partial class ProfileViewModel : ViewModelBase
    {
        [ObservableProperty] private string _fullName = "Juan dela Cruz";
        [ObservableProperty] private string _birthDate = "January 1, 2000";
        [ObservableProperty] private string _contactNumber = "09123456789";
        [ObservableProperty] private string _email = "juan.delacruz@email.com";
        [ObservableProperty] private string _address = "San Pedro, Laguna, Philippines";
        [ObservableProperty] private string _education = "Polytechnic University of the Philippines";
        [ObservableProperty] private string _skills = "C#, SQL, Git";
        [ObservableProperty] private string _workExperience = "No professional experience yet. Fresh Graduate.";
        
        [ObservableProperty] private bool _isReadOnly = true;
        [ObservableProperty] private string _editButtonText = "📝 Edit Profile";

        public string AvatarInitials => GetInitials(FullName);

        [RelayCommand]
        private void ToggleEditMode()
        {
            IsReadOnly = !IsReadOnly;
            EditButtonText = IsReadOnly ? "📝 Edit Profile" : "💾 Save Changes";
        }

        private static string GetInitials(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "?";
            var parts = name.Split(' ');
            if (parts.Length >= 2) return $"{parts[0][0]}{parts[^1][0]}".ToUpper();
            return name.Length > 0 ? name[0].ToString().ToUpper() : "?";
        }
    }
}