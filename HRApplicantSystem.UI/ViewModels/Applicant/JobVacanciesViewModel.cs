using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRApplicantSystem.Data;
using HRApplicantSystem.Data.Models;
using HRApplicantSystem.Data.Repositories;
using HRApplicantSystem.Services.Interfaces;
using HRApplicantSystem.Shared.Enums;
using HRApplicantSystem.Shared.Helpers;

namespace HRApplicantSystem.UI.ViewModels.Applicant;

public partial class JobVacanciesViewModel : ViewModelBase
{
    private readonly IJobVacancyService _jobVacancyService;
    private readonly IApplicationService _applicationService;
    private readonly MainWindowViewModel? _mainViewModel;

    public ObservableCollection<JobVacancy> Vacancies { get; } = new();

    // Documents that the applicant attaches as part of this application.
    // HR will see these when reviewing the application.
    public ObservableCollection<ApplicantDocument> AttachedDocuments { get; } = new();

    public ObservableCollection<RequirementTypeOption> RequirementTypes { get; } = new()
    {
        new() { Id = "79263f6b-68c0-11f1-baa4-f894c21dce1e", Name = "Resume" },
        new() { Id = "79264256-68c0-11f1-baa4-f894c21dce1e", Name = "ID" },
        new() { Id = "7926434c-68c0-11f1-baa4-f894c21dce1e", Name = "Transcript" },
        new() { Id = "79264402-68c0-11f1-baa4-f894c21dce1e", Name = "Certificate" },
        new() { Id = "7926447c-68c0-11f1-baa4-f894c21dce1e", Name = "Other" }
    };

    // Required document types that must be uploaded before submitting.
    private static readonly string[] RequiredTypeIds =
    {
        "79263f6b-68c0-11f1-baa4-f894c21dce1e", // Resume
        "79264256-68c0-11f1-baa4-f894c21dce1e", // ID
        "7926434c-68c0-11f1-baa4-f894c21dce1e", // Transcript
        "79264402-68c0-11f1-baa4-f894c21dce1e"  // Certificate
    };

    [ObservableProperty] private JobVacancy? selectedJob;
    [ObservableProperty] private string searchText = "";
    [ObservableProperty] private string message = "";
    [ObservableProperty] private bool hasMessage;
    [ObservableProperty] private bool canApply;

    // Controls which panel is visible: "list" = job list, "apply" = application + documents panel
    [ObservableProperty] private bool isInApplyMode = false;

    [ObservableProperty] private RequirementTypeOption? selectedRequirementType;
    [ObservableProperty] private ApplicantDocument? selectedDocument;
    [ObservableProperty] private string selectedFilePath = "";
    [ObservableProperty] private int missingDocumentCount;
    [ObservableProperty] private string missingDocumentLabel = "";

    // True once all required docs are uploaded — enables the final Submit button
    [ObservableProperty] private bool canSubmitApplication = false;

    public JobVacanciesViewModel(
        IJobVacancyService jobVacancyService,
        IApplicationService applicationService,
        MainWindowViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        _jobVacancyService = jobVacancyService;
        _applicationService = applicationService;
        _ = LoadJobVacanciesAsync();
    }

    public async Task LoadJobVacanciesAsync()
    {
        var jobs = await _jobVacancyService.GetOpenJobsAsync();
        Vacancies.Clear();
        foreach (var job in jobs)
            Vacancies.Add(job);
    }

    partial void OnSearchTextChanged(string value) => _ = SearchAsync(value);

    public async Task SearchAsync(string keyword)
    {
        var jobs = string.IsNullOrWhiteSpace(keyword)
            ? await _jobVacancyService.GetOpenJobsAsync()
            : await _jobVacancyService.SearchJobsAsync(keyword);

        Vacancies.Clear();
        foreach (var job in jobs)
            Vacancies.Add(job);
    }

    partial void OnSelectedJobChanged(JobVacancy? value)
    {
        CanApply = value != null;
    }

    // Step 1: applicant clicks "Apply Now" → switch to the documents panel
    [RelayCommand]
    public async Task EnterApplyModeAsync()
    {
        if (SelectedJob == null) return;

        // Verify the applicant has a profile before allowing them to apply
        try
        {
            var db = new DbContext(AppConfig.ConnectionString);
            var applicantRepo = new ApplicantRepository(db);
            var applicant = await applicantRepo.GetByAccountIdAsync(
                SessionManager.CurrentUserId ?? string.Empty);

            if (applicant == null)
            {
                Message = "Please complete your profile before applying.";
                HasMessage = true;
                return;
            }

            // Check if applicant already applied to this job
            var appRepo = new ApplicationRepository(db);
            bool alreadyApplied = await appRepo.ExistsAsync(applicant.ApplicantId, SelectedJob.VacancyId);
            if (alreadyApplied)
            {
                Message = "You have already applied for this job.";
                HasMessage = true;
                return;
            }

            // Load any documents the applicant has already uploaded globally
            await LoadApplicantDocumentsAsync(applicant.ApplicantId);
        }
        catch (Exception ex)
        {
            Message = ex.Message;
            HasMessage = true;
            return;
        }

        IsInApplyMode = true;
        HasMessage = false;
        Message = "";
    }

    // Load documents already on record for this applicant (from previous uploads if any)
    private async Task LoadApplicantDocumentsAsync(string applicantId)
    {
        var db = new DbContext(AppConfig.ConnectionString);
        var docRepo = new ApplicantDocumentRepository(db);
        var docs = await docRepo.GetByApplicantIdAsync(applicantId);

        AttachedDocuments.Clear();
        foreach (var d in docs)
            AttachedDocuments.Add(d);

        UpdateMissingCount();
    }

    private void UpdateMissingCount()
    {
        var uploadedIds = AttachedDocuments
            .Where(d => d.Status == DocumentStatus.Submitted)
            .Select(d => d.RequirementTypeId)
            .ToHashSet();

        MissingDocumentCount = RequiredTypeIds.Count(id => !uploadedIds.Contains(id));
        MissingDocumentLabel = MissingDocumentCount == 0
            ? "✅ All required documents uploaded"
            : $"⚠️ {MissingDocumentCount} required document(s) still missing";

        // Only allow final submission once all required docs are present
        CanSubmitApplication = MissingDocumentCount == 0;
    }

    // Browse and upload a document as part of this application
    [RelayCommand]
    public async Task UploadDocumentAsync()
    {
        try
        {
            if (SelectedRequirementType == null)
            {
                Message = "Please select a document type first.";
                HasMessage = true;
                return;
            }

            if (Avalonia.Application.Current?.ApplicationLifetime
                is not IClassicDesktopStyleApplicationLifetime desktop)
                return;

            var files = await desktop.MainWindow!
                .StorageProvider
                .OpenFilePickerAsync(new() { AllowMultiple = false, Title = "Select Document" });

            if (files.Count == 0) return;

            var fullPath = files[0].Path.LocalPath;
            if (!File.Exists(fullPath))
            {
                Message = "File not found.";
                HasMessage = true;
                return;
            }

            var ext = Path.GetExtension(fullPath).ToLower();
            if (ext == ".zip")
            {
                Message = "ZIP files are not allowed.";
                HasMessage = true;
                return;
            }

            var db = new DbContext(AppConfig.ConnectionString);
            var applicantRepo = new ApplicantRepository(db);
            var docRepo = new ApplicantDocumentRepository(db);

            var applicant = await applicantRepo.GetByAccountIdAsync(
                SessionManager.CurrentUserId ?? "");
            if (applicant == null) { Message = "Applicant not found."; HasMessage = true; return; }

            var requirementId = SelectedRequirementType.Id?.Trim();
            if (string.IsNullOrWhiteSpace(requirementId) || requirementId.Length != 36)
            {
                Message = "Invalid document type.";
                HasMessage = true;
                return;
            }

            await docRepo.CreateAsync(new ApplicantDocument
            {
                ApplicantId = applicant.ApplicantId,
                RequirementTypeId = requirementId,
                FilePath = fullPath,
                Status = DocumentStatus.Submitted,
                SubmittedAt = DateTime.Now
            });

            Message = "Document uploaded successfully.";
            HasMessage = true;
            SelectedRequirementType = null;

            // Reload documents and update the missing count
            await LoadApplicantDocumentsAsync(applicant.ApplicantId);
        }
        catch (Exception ex) { Message = ex.Message; HasMessage = true; }
    }

    [RelayCommand]
    public async Task RemoveDocumentAsync()
    {
        if (SelectedDocument == null) return;
        try
        {
            var db = new DbContext(AppConfig.ConnectionString);
            var docRepo = new ApplicantDocumentRepository(db);
            await docRepo.DeleteAsync(SelectedDocument.DocumentId);

            var applicantRepo = new ApplicantRepository(db);
            var applicant = await applicantRepo.GetByAccountIdAsync(
                SessionManager.CurrentUserId ?? "");
            if (applicant != null)
                await LoadApplicantDocumentsAsync(applicant.ApplicantId);

            Message = "Document removed.";
            HasMessage = true;
        }
        catch (Exception ex) { Message = ex.Message; HasMessage = true; }
    }

    // Step 2: final submit — creates the application record and links the uploaded docs
    [RelayCommand]
    public async Task SubmitApplicationAsync()
    {
        if (SelectedJob == null) return;

        try
        {
            var db = new DbContext(AppConfig.ConnectionString);
            var applicantRepo = new ApplicantRepository(db);
            var applicant = await applicantRepo.GetByAccountIdAsync(
                SessionManager.CurrentUserId ?? string.Empty);

            if (applicant == null)
            {
                Message = "Please complete your profile first!";
                HasMessage = true;
                return;
            }

            bool success = await _applicationService.SubmitApplicationAsync(
                applicant.ApplicantId,
                SelectedJob.VacancyId);

            if (success)
            {
                Message = "✅ Application submitted successfully! HR will review your documents.";
                HasMessage = true;
                IsInApplyMode = false;
                SelectedJob = null;
                AttachedDocuments.Clear();
                MissingDocumentLabel = "";
                await LoadJobVacanciesAsync();
            }
            else
            {
                Message = "You may have already applied or the job is closed.";
                HasMessage = true;
            }
        }
        catch (Exception ex) { Message = ex.Message; HasMessage = true; }
    }

    // Cancel the application flow and go back to the job list
    [RelayCommand]
    public void CancelApply()
    {
        IsInApplyMode = false;
        AttachedDocuments.Clear();
        MissingDocumentLabel = "";
        HasMessage = false;
        Message = "";
    }

    [RelayCommand]
    private void GoBack() => _mainViewModel?.NavigateToApplicantDashboard();

    public class RequirementTypeOption
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
    }
}
