using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRApplicantSystem.Data.Models;
using HRApplicantSystem.Services.Interfaces;
using HRApplicantSystem.Shared.Enums;
using HRApplicantSystem.Shared.Helpers;

namespace HRApplicantSystem.UI.ViewModels.HR;

/// <summary>
/// ViewModel for HR Job Vacancy Management module.
/// Handles CRUD operations and status management for job vacancies.
/// </summary>
public partial class JobVacancyMgmtViewModel : ViewModelBase
{
    private readonly IJobVacancyMgmtService _jobVacancyMgmtService;

    /// <summary>
    /// Collection of all job vacancies (open and closed).
    /// Bound to ListBox in JobVacancyMgmtView.
    /// </summary>
    public ObservableCollection<JobVacancy> Vacancies { get; } = new();

    [ObservableProperty]
    private JobVacancy? selectedJobVacancy;

    [ObservableProperty]
    private string message = "";

    [ObservableProperty]
    private bool hasMessage;

    [ObservableProperty]
    private bool isLoading;

    public JobVacancyMgmtViewModel(IJobVacancyMgmtService jobVacancyMgmtService)
    {
        _jobVacancyMgmtService = jobVacancyMgmtService;
        Message = "Initializing...";
        HasMessage = true;

        // Fire-and-forget with proper async handling
        _ = LoadJobVacanciesAsync();
    }

    /// <summary>
    /// Loads all job vacancies from the database.
    /// </summary>
    public async Task LoadJobVacanciesAsync()
    {
        try
        {
            IsLoading = true;
            Vacancies.Clear();

            var jobs = await _jobVacancyMgmtService.GetAllJobsAsync();
            var jobList = jobs.ToList();

            foreach (var job in jobList)
            {
                Vacancies.Add(job);
            }

            Message = $"Loaded {jobList.Count} job vacancies";
            HasMessage = true;
        }
        catch (Exception ex)
        {
            Message = $"Error loading vacancies: {ex.Message}";
            HasMessage = true;
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Adds a new job vacancy to the database.
    /// </summary>
    [RelayCommand]
    public async Task AddJobVacancyAsync(JobVacancy? vacancy)
    {
        if (vacancy == null)
        {
            Message = "Please provide job vacancy details.";
            HasMessage = true;
            return;
        }

        try
        {
            vacancy.VacancyId = Guid.NewGuid().ToString();
            vacancy.CreatedAt = DateTime.Now;
            vacancy.Status = VacancyStatus.Open;

            bool success = await _jobVacancyMgmtService.CreateJobVacancyAsync(vacancy);

            if (success)
            {
                Vacancies.Add(vacancy);
                Message = "Job vacancy added successfully!";
            }
            else
            {
                Message = "Failed to add job vacancy.";
            }

            HasMessage = true;
        }
        catch (Exception ex)
        {
            Message = $"Error adding vacancy: {ex.Message}";
            HasMessage = true;
        }
    }

    /// <summary>
    /// Updates the selected job vacancy.
    /// </summary>
    [RelayCommand]
    public async Task EditJobVacancyAsync(JobVacancy? vacancy)
    {
        if (vacancy == null)
        {
            Message = "Please select a vacancy to edit.";
            HasMessage = true;
            return;
        }

        try
        {
            bool success = await _jobVacancyMgmtService.UpdateJobVacancyAsync(vacancy);

            if (success)
            {
                Message = "Job vacancy updated successfully!";
            }
            else
            {
                Message = "Failed to update job vacancy.";
            }

            HasMessage = true;
        }
        catch (Exception ex)
        {
            Message = $"Error updating vacancy: {ex.Message}";
            HasMessage = true;
        }
    }

    /// <summary>
    /// Closes the selected job vacancy (changes status to Closed).
    /// </summary>
    [RelayCommand]
    public async Task CloseJobVacancyAsync()
    {
        if (SelectedJobVacancy == null)
        {
            Message = "Please select a vacancy to close.";
            HasMessage = true;
            return;
        }

        try
        {
            bool success = await _jobVacancyMgmtService.CloseJobVacancyAsync(
                SelectedJobVacancy.VacancyId);

            if (success)
            {
                SelectedJobVacancy.Status = VacancyStatus.Closed;
                Message = "Job vacancy closed successfully!";
            }
            else
            {
                Message = "Failed to close job vacancy.";
            }

            HasMessage = true;
        }
        catch (Exception ex)
        {
            Message = $"Error closing vacancy: {ex.Message}";
            HasMessage = true;
        }
    }

    /// <summary>
    /// Reopens a closed job vacancy (changes status back to Open).
    /// </summary>
    [RelayCommand]
    public async Task ReopenJobVacancyAsync()
    {
        if (SelectedJobVacancy == null)
        {
            Message = "Please select a vacancy to reopen.";
            HasMessage = true;
            return;
        }

        try
        {
            bool success = await _jobVacancyMgmtService.OpenJobVacancyAsync(
                SelectedJobVacancy.VacancyId);

            if (success)
            {
                SelectedJobVacancy.Status = VacancyStatus.Open;
                Message = "Job vacancy reopened successfully!";
            }
            else
            {
                Message = "Failed to reopen job vacancy.";
            }

            HasMessage = true;
        }
        catch (Exception ex)
        {
            Message = $"Error reopening vacancy: {ex.Message}";
            HasMessage = true;
        }
    }
}
