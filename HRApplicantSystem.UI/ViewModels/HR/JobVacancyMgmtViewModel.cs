using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRApplicantSystem.Data;
using HRApplicantSystem.Data.Models;
using HRApplicantSystem.Data.Repositories;
using HRApplicantSystem.Services.Interfaces;
using HRApplicantSystem.Shared.Enums;

namespace HRApplicantSystem.UI.ViewModels.HR;

public partial class JobVacancyMgmtViewModel : ViewModelBase
{
    private readonly IJobVacancyMgmtService _jobVacancyMgmtService;
    private readonly MainWindowViewModel? _mainViewModel;

    public ObservableCollection<JobVacancy> Vacancies { get; } = new();
    public ObservableCollection<Department> Departments { get; } = new();

    [ObservableProperty] private JobVacancy? selectedJobVacancy;
    [ObservableProperty] private string message = "";
    [ObservableProperty] private bool hasMessage;
    [ObservableProperty] private bool isLoading;

    // Controls whether the Add/Edit form panel is visible
    [ObservableProperty] private bool isFormVisible = false;

    // True when editing an existing record; false when adding a new one
    [ObservableProperty] private bool isEditing = false;

    // Form field bindings
    [ObservableProperty] private string formPositionTitle = "";
    [ObservableProperty] private string formQualifications = "";
    [ObservableProperty] private string formEmploymentType = "Full-Time";
    [ObservableProperty] private Department? formDepartment;
    [ObservableProperty] private string formTitle = "Add New Job Vacancy";

    public ObservableCollection<string> EmploymentTypes { get; } = new()
    {
        "Full-Time", "Part-Time", "Contract", "Internship", "Temporary"
    };

    public JobVacancyMgmtViewModel(IJobVacancyMgmtService jobVacancyMgmtService)
    {
        _jobVacancyMgmtService = jobVacancyMgmtService;
        _ = InitAsync();
    }

    public JobVacancyMgmtViewModel(IJobVacancyMgmtService jobVacancyMgmtService, MainWindowViewModel mainViewModel)
    {
        _jobVacancyMgmtService = jobVacancyMgmtService;
        _mainViewModel = mainViewModel;
        _ = InitAsync();
    }

    private async Task InitAsync()
    {
        await LoadDepartmentsAsync();
        await LoadJobVacanciesAsync();
    }

    private async Task LoadDepartmentsAsync()
    {
        try
        {
            var db = new DbContext(AppConfig.ConnectionString);
            var deptRepo = new DepartmentRepository(db);
            var depts = await deptRepo.GetAllAsync();
            Departments.Clear();
            foreach (var d in depts) Departments.Add(d);
        }
        catch (Exception ex) { Message = $"Could not load departments: {ex.Message}"; HasMessage = true; }
    }

    public async Task LoadJobVacanciesAsync()
    {
        try
        {
            IsLoading = true;
            var jobs = await _jobVacancyMgmtService.GetAllJobsAsync();
            Vacancies.Clear();
            foreach (var job in jobs.OrderByDescending(j => j.CreatedAt))
                Vacancies.Add(job);
        }
        catch (Exception ex) { Message = $"Error loading vacancies: {ex.Message}"; HasMessage = true; }
        finally { IsLoading = false; }
    }

    // Open the form pre-filled for editing the selected vacancy
    [RelayCommand]
    public void OpenEditForm()
    {
        if (SelectedJobVacancy == null)
        {
            Message = "Please select a vacancy to edit.";
            HasMessage = true;
            return;
        }

        FormPositionTitle  = SelectedJobVacancy.PositionTitle;
        FormQualifications = SelectedJobVacancy.Qualifications ?? "";
        FormEmploymentType = SelectedJobVacancy.EmploymentType;
        FormDepartment     = Departments.FirstOrDefault(d => d.DepartmentId == SelectedJobVacancy.DepartmentId);
        FormTitle          = "Edit Job Vacancy";
        IsEditing          = true;
        IsFormVisible      = true;
        HasMessage         = false;
    }

    // Open the form blank for adding a new vacancy
    [RelayCommand]
    public void OpenAddForm()
    {
        FormPositionTitle  = "";
        FormQualifications = "";
        FormEmploymentType = "Full-Time";
        FormDepartment     = Departments.FirstOrDefault();
        FormTitle          = "Add New Job Vacancy";
        IsEditing          = false;
        IsFormVisible      = true;
        HasMessage         = false;
    }

    // Dismiss the form without saving
    [RelayCommand]
    public void CancelForm()
    {
        IsFormVisible = false;
        HasMessage    = false;
    }

    // Save — creates or updates depending on IsEditing
    [RelayCommand]
    public async Task SaveVacancyAsync()
    {
        if (string.IsNullOrWhiteSpace(FormPositionTitle))
        {
            Message = "Position title is required.";
            HasMessage = true;
            return;
        }
        if (FormDepartment == null)
        {
            Message = "Please select a department.";
            HasMessage = true;
            return;
        }

        try
        {
            bool success;

            if (IsEditing && SelectedJobVacancy != null)
            {
                // Edit existing record
                SelectedJobVacancy.PositionTitle  = FormPositionTitle.Trim();
                SelectedJobVacancy.Qualifications = FormQualifications.Trim();
                SelectedJobVacancy.EmploymentType = FormEmploymentType;
                SelectedJobVacancy.DepartmentId   = FormDepartment.DepartmentId;

                success = await _jobVacancyMgmtService.UpdateJobVacancyAsync(SelectedJobVacancy);
                Message = success ? "✅ Job vacancy updated successfully." : "Failed to update vacancy.";
            }
            else
            {
                // Create new record
                var newVacancy = new JobVacancy
                {
                    VacancyId     = Guid.NewGuid().ToString(),
                    PositionTitle  = FormPositionTitle.Trim(),
                    Qualifications = FormQualifications.Trim(),
                    EmploymentType = FormEmploymentType,
                    DepartmentId   = FormDepartment.DepartmentId,
                    Status         = VacancyStatus.Open,
                    CreatedAt      = DateTime.Now
                };

                success = await _jobVacancyMgmtService.CreateJobVacancyAsync(newVacancy);
                Message = success ? "✅ Job vacancy added successfully." : "Failed to add vacancy.";
            }

            HasMessage    = true;
            IsFormVisible = success ? false : true;

            if (success) await LoadJobVacanciesAsync();
        }
        catch (Exception ex) { Message = $"Error: {ex.Message}"; HasMessage = true; }
    }

    [RelayCommand]
    public async Task CloseJobVacancyAsync()
    {
        if (SelectedJobVacancy == null) { Message = "Please select a vacancy to close."; HasMessage = true; return; }
        try
        {
            bool ok = await _jobVacancyMgmtService.CloseJobVacancyAsync(SelectedJobVacancy.VacancyId);
            Message = ok ? "✅ Vacancy closed." : "Failed to close vacancy.";
            HasMessage = true;
            if (ok) await LoadJobVacanciesAsync();
        }
        catch (Exception ex) { Message = ex.Message; HasMessage = true; }
    }

    [RelayCommand]
    public async Task ReopenJobVacancyAsync()
    {
        if (SelectedJobVacancy == null) { Message = "Please select a vacancy to reopen."; HasMessage = true; return; }
        try
        {
            bool ok = await _jobVacancyMgmtService.OpenJobVacancyAsync(SelectedJobVacancy.VacancyId);
            Message = ok ? "✅ Vacancy reopened." : "Failed to reopen vacancy.";
            HasMessage = true;
            if (ok) await LoadJobVacanciesAsync();
        }
        catch (Exception ex) { Message = ex.Message; HasMessage = true; }
    }

    [RelayCommand]
    private void GoBack() => _mainViewModel?.NavigateToHRDashboard();
}
