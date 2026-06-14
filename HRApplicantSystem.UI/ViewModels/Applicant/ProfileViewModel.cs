using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using HRApplicantSystem.Data;
using HRApplicantSystem.Data.Models;
using HRApplicantSystem.Data.Repositories;
using HRApplicantSystem.Shared.Helpers;

namespace HRApplicantSystem.UI.ViewModels.Applicant;

public partial class ProfileViewModel : ViewModelBase
{
    private readonly MainWindowViewModel? _mainViewModel;
    private string? _applicantId;

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

    private string _address = "";
    public string Address
    {
        get => _address;
        set { _address = value; OnPropertyChanged(); }
    }

    private string _education = "";
    public string Education
    {
        get => _education;
        set { _education = value; OnPropertyChanged(); }
    }

    private string _workExperience = "";
    public string WorkExperience
    {
        get => _workExperience;
        set { _workExperience = value; OnPropertyChanged(); }
    }

    private string _skills = "";
    public string Skills
    {
        get => _skills;
        set { _skills = value; OnPropertyChanged(); }
    }

    private string _statusMessage = "";
    public string StatusMessage
    {
        get => _statusMessage;
        set { _statusMessage = value; OnPropertyChanged(); }
    }

    private bool _hasMessage;
    public bool HasMessage
    {
        get => _hasMessage;
        set { _hasMessage = value; OnPropertyChanged(); }
    }

    public class EducationItem
    {
        public string Degree { get; set; } = string.Empty;
        public string Institution { get; set; } = string.Empty;
        public string GraduationYear { get; set; } = string.Empty;
    }

    public class ExperienceItem
    {
        public string JobTitle { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
    }

    public ObservableCollection<dynamic> EducationList { get; set; } = new();
    public ObservableCollection<dynamic> WorkExperienceList { get; set; } = new();

    public ProfileViewModel() { _ = LoadProfileAsync(); }

    public ProfileViewModel(MainWindowViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        _ = LoadProfileAsync();
    }

    public async Task LoadProfileAsync()
    {
        try
        {
            var db = new DbContext(AppConfig.ConnectionString);
            var repo = new ApplicantRepository(db);
            var applicant = await repo.GetByAccountIdAsync(SessionManager.CurrentUserId ?? string.Empty);
            if (applicant == null) return;

            _applicantId = applicant.ApplicantId;
            var nameParts = applicant.FullName?.Split(' ') ?? [];
            FirstName = nameParts.Length > 0 ? nameParts[0] : "";
            LastName = nameParts.Length > 1 ? string.Join(" ", nameParts[1..]) : "";
            Phone = applicant.ContactNo ?? "";
            Address = applicant.Address ?? "";
            Education = applicant.Education ?? "";
            WorkExperience = applicant.WorkExperience ?? "";
            Skills = applicant.Skills ?? "";
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            HasMessage = true;
        }
    }

    public async Task SaveChangesAsync()
    {
        try
        {
            var db = new DbContext(AppConfig.ConnectionString);
            var repo = new ApplicantRepository(db);

            if (_applicantId == null)
            {
                var newApplicant = new Data.Models.Applicant
                {
                    AccountId = SessionManager.CurrentUserId ?? string.Empty,
                    FullName = $"{FirstName} {LastName}".Trim(),
                    ContactNo = Phone,
                    Address = Address,
                    Education = Education,
                    WorkExperience = WorkExperience,
                    Skills = Skills
                };
                await repo.CreateAsync(newApplicant);
            }
            else
            {
                var applicant = await repo.GetByIdAsync(_applicantId);
                if (applicant == null) return;
                applicant.FullName = $"{FirstName} {LastName}".Trim();
                applicant.ContactNo = Phone;
                applicant.Address = Address;
                applicant.Education = Education;
                applicant.WorkExperience = WorkExperience;
                applicant.Skills = Skills;
                await repo.UpdateAsync(applicant);
            }

            StatusMessage = "Profile saved successfully!";
            HasMessage = true;
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            HasMessage = true;
        }
    }

    [RelayCommand]
    private async Task Save() => await SaveChangesAsync();

    [RelayCommand]
    private void GoBack() => _mainViewModel?.NavigateToApplicantDashboard();
}