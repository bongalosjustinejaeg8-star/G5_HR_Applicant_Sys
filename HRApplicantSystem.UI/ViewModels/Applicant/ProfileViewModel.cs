using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using HRApplicantSystem.Data;
using HRApplicantSystem.Data.Repositories;
using HRApplicantSystem.Shared.Helpers;

namespace HRApplicantSystem.UI.ViewModels.Applicant;

public partial class ProfileViewModel : ViewModelBase
{
    private readonly MainWindowViewModel? _mainViewModel;
    private string? _applicantId;

    // ── Personal Info ──────────────────────────────────
    private string _firstName = "";
    public string FirstName { get => _firstName; set { _firstName = value; OnPropertyChanged(); } }

    private string _lastName = "";
    public string LastName { get => _lastName; set { _lastName = value; OnPropertyChanged(); } }

    private string _email = "";
    public string Email { get => _email; set { _email = value; OnPropertyChanged(); } }

    private string _phone = "";
    public string Phone { get => _phone; set { _phone = value; OnPropertyChanged(); } }

    private string _address = "";
    public string Address { get => _address; set { _address = value; OnPropertyChanged(); } }

    private string _skills = "";
    public string Skills { get => _skills; set { _skills = value; OnPropertyChanged(); } }

    // ── Status ─────────────────────────────────────────
    private string _statusMessage = "";
    public string StatusMessage { get => _statusMessage; set { _statusMessage = value; OnPropertyChanged(); } }

    private bool _hasMessage;
    public bool HasMessage { get => _hasMessage; set { _hasMessage = value; OnPropertyChanged(); } }

    // ── Education add form ─────────────────────────────
    private string _newDegree = "";
    public string NewDegree { get => _newDegree; set { _newDegree = value; OnPropertyChanged(); } }

    private string _newInstitution = "";
    public string NewInstitution { get => _newInstitution; set { _newInstitution = value; OnPropertyChanged(); } }

    private string _newGraduationYear = "";
    public string NewGraduationYear { get => _newGraduationYear; set { _newGraduationYear = value; OnPropertyChanged(); } }

    // ── Experience add form ────────────────────────────
    private string _newJobTitle = "";
    public string NewJobTitle { get => _newJobTitle; set { _newJobTitle = value; OnPropertyChanged(); } }

    private string _newCompany = "";
    public string NewCompany { get => _newCompany; set { _newCompany = value; OnPropertyChanged(); } }

    // ── Lists ──────────────────────────────────────────
    public ObservableCollection<EducationItem> EducationList { get; set; } = new();
    public ObservableCollection<ExperienceItem> WorkExperienceList { get; set; } = new();

    // ── Nested classes ─────────────────────────────────
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

    // ── Constructors ───────────────────────────────────
    public ProfileViewModel() { _ = LoadProfileAsync(); }

    public ProfileViewModel(MainWindowViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        _ = LoadProfileAsync();
    }

    // ── Load ───────────────────────────────────────────
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
            Skills = applicant.Skills ?? "";

            // parse education from stored string "Degree|Institution|Year;Degree|Institution|Year"
            EducationList.Clear();
            if (!string.IsNullOrWhiteSpace(applicant.Education))
            {
                foreach (var entry in applicant.Education.Split(';', StringSplitOptions.RemoveEmptyEntries))
                {
                    var parts = entry.Split('|');
                    EducationList.Add(new EducationItem
                    {
                        Degree = parts.Length > 0 ? parts[0] : "",
                        Institution = parts.Length > 1 ? parts[1] : "",
                        GraduationYear = parts.Length > 2 ? parts[2] : ""
                    });
                }
            }

            // parse experience from stored string "JobTitle|Company;JobTitle|Company"
            WorkExperienceList.Clear();
            if (!string.IsNullOrWhiteSpace(applicant.WorkExperience))
            {
                foreach (var entry in applicant.WorkExperience.Split(';', StringSplitOptions.RemoveEmptyEntries))
                {
                    var parts = entry.Split('|');
                    WorkExperienceList.Add(new ExperienceItem
                    {
                        JobTitle = parts.Length > 0 ? parts[0] : "",
                        CompanyName = parts.Length > 1 ? parts[1] : ""
                    });
                }
            }
        }
        catch (Exception ex) { StatusMessage = ex.Message; HasMessage = true; }
    }

    // ── Education commands ─────────────────────────────
    [RelayCommand]
    private void AddEducation()
    {
        if (string.IsNullOrWhiteSpace(NewDegree)) return;
        EducationList.Add(new EducationItem
        {
            Degree = NewDegree,
            Institution = NewInstitution,
            GraduationYear = NewGraduationYear
        });
        NewDegree = "";
        NewInstitution = "";
        NewGraduationYear = "";
    }

    [RelayCommand]
    private void RemoveEducation(EducationItem item) => EducationList.Remove(item);

    // ── Experience commands ────────────────────────────
    [RelayCommand]
    private void AddExperience()
    {
        if (string.IsNullOrWhiteSpace(NewJobTitle)) return;
        WorkExperienceList.Add(new ExperienceItem
        {
            JobTitle = NewJobTitle,
            CompanyName = NewCompany
        });
        NewJobTitle = "";
        NewCompany = "";
    }

    [RelayCommand]
    private void RemoveExperience(ExperienceItem item) => WorkExperienceList.Remove(item);

    // ── Save ───────────────────────────────────────────
    public async Task SaveChangesAsync()
    {
        try
        {
            var db = new DbContext(AppConfig.ConnectionString);
            var repo = new ApplicantRepository(db);

            // serialize lists back to strings
            var educationString = string.Join(";", EducationList.Select(e => $"{e.Degree}|{e.Institution}|{e.GraduationYear}"));
            var experienceString = string.Join(";", WorkExperienceList.Select(e => $"{e.JobTitle}|{e.CompanyName}"));

            if (_applicantId == null)
            {
                var newApplicant = new HRApplicantSystem.Data.Models.Applicant
                {
                    AccountId = SessionManager.CurrentUserId ?? string.Empty,
                    FullName = $"{FirstName} {LastName}".Trim(),
                    ContactNo = Phone,
                    Address = Address,
                    Education = educationString,
                    WorkExperience = experienceString,
                    Skills = Skills
                };
                await repo.CreateAsync(newApplicant);
                var created = await repo.GetByAccountIdAsync(SessionManager.CurrentUserId ?? string.Empty);
                _applicantId = created?.ApplicantId;
            }
            else
            {
                var applicant = await repo.GetByIdAsync(_applicantId);
                if (applicant == null) return;
                applicant.FullName = $"{FirstName} {LastName}".Trim();
                applicant.ContactNo = Phone;
                applicant.Address = Address;
                applicant.Education = educationString;
                applicant.WorkExperience = experienceString;
                applicant.Skills = Skills;
                await repo.UpdateAsync(applicant);
            }

            StatusMessage = "Profile saved successfully!";
            HasMessage = true;
        }
        catch (Exception ex) { StatusMessage = ex.Message; HasMessage = true; }
    }

    [RelayCommand] private async Task Save() => await SaveChangesAsync();
    [RelayCommand] private void GoBack() => _mainViewModel?.NavigateToApplicantDashboard();
}