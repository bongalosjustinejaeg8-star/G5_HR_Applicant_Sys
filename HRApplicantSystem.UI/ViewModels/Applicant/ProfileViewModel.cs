using System.Collections.ObjectModel;
using HRApplicantSystem.Shared.Helpers;

namespace HRApplicantSystem.UI.ViewModels;

public class ProfileViewModel : ViewModelBase
{
    private string _firstName = "";
    public string FirstName
    {
        get => _firstName;
        set { _firstName = value; OnPropertyChanged(); }
    }

    private string _lastName = "";
    public string LastName
    {
        get => _lastName;
        set { _lastName = value; OnPropertyChanged(); }
    }

    private string _email = "";
    public string Email
    {
        get => _email;
        set { _email = value; OnPropertyChanged(); }
    }

    private string _phone = "";
    public string Phone
    {
        get => _phone;
        set { _phone = value; OnPropertyChanged(); }
    }

    public ObservableCollection<dynamic> EducationList { get; set; } = new();
    public ObservableCollection<dynamic> WorkExperienceList { get; set; } = new();

    public ProfileViewModel()
    {
        // TODO: Initialize profile
    }

    public async Task LoadProfileAsync()
    {
        // TODO: Load applicant profile
    }

    public async Task SaveChangesAsync()
    {
        // TODO: Save profile changes
    }
}
