using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRApplicantSystem.Data;
using HRApplicantSystem.Data.Models;
using HRApplicantSystem.Data.Repositories;
using HRApplicantSystem.Shared.Enums;
using HRApplicantSystem.Shared.Helpers;

namespace HRApplicantSystem.UI.ViewModels.Applicant;

public partial class MyDocumentsViewModel : ViewModelBase
{
    private readonly MainWindowViewModel? _mainViewModel;

    public ObservableCollection<ApplicantDocument> Documents { get; } = new();

    public ObservableCollection<RequirementTypeOption> RequirementTypes { get; } = new()
    {
        new() { Id = "79263f6b-68c0-11f1-baa4-f894c21dce1e", Name = "Resume" },
        new() { Id = "79264256-68c0-11f1-baa4-f894c21dce1e", Name = "ID" },
        new() { Id = "7926434c-68c0-11f1-baa4-f894c21dce1e", Name = "Transcript" },
        new() { Id = "79264402-68c0-11f1-baa4-f894c21dce1e", Name = "Certificate" },
        new() { Id = "7926447c-68c0-11f1-baa4-f894c21dce1e", Name = "Other" }
    };

    [ObservableProperty]
    private RequirementTypeOption? selectedRequirementType;

    [ObservableProperty]
    private ApplicantDocument? selectedDocument;

    [ObservableProperty]
    private string filePath = "";

    [ObservableProperty]
    private string statusMessage = "";

    [ObservableProperty]
    private bool hasMessage;

    public MyDocumentsViewModel()
    {
        _ = LoadDocumentsAsync();
    }

    public MyDocumentsViewModel(MainWindowViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        _ = LoadDocumentsAsync();
    }

    public async Task LoadDocumentsAsync()
    {
        try
        {
            var db = new DbContext(AppConfig.ConnectionString);

            var applicantRepo = new ApplicantRepository(db);
            var docRepo = new ApplicantDocumentRepository(db);

            var applicant =
                await applicantRepo.GetByAccountIdAsync(SessionManager.CurrentUserId ?? "");

            if (applicant == null)
                return;

            Documents.Clear();

            var docs =
                await docRepo.GetByApplicantIdAsync(applicant.ApplicantId);

            foreach (var d in docs)
                Documents.Add(d);
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            HasMessage = true;
        }
    }

    [RelayCommand]
    public async Task SelectAndUploadDocumentAsync()
    {
        try
        {
            if (SelectedRequirementType == null)
            {
                StatusMessage = "Please select a document type.";
                HasMessage = true;
                return;
            }

            if (Avalonia.Application.Current?.ApplicationLifetime
                is not IClassicDesktopStyleApplicationLifetime desktop)
                return;

            var files = await desktop.MainWindow!
                .StorageProvider
                .OpenFilePickerAsync(new()
                {
                    AllowMultiple = false,
                    Title = "Select Document"
                });

            if (files.Count == 0)
                return;

            var fullPath = files[0].Path.LocalPath;

            if (!File.Exists(fullPath))
            {
                StatusMessage = "File not found.";
                HasMessage = true;
                return;
            }

            var ext = Path.GetExtension(fullPath).ToLower();

            if (ext == ".zip")
            {
                StatusMessage = "ZIP files are not allowed.";
                HasMessage = true;
                return;
            }

            FilePath = fullPath;

            await UploadAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            HasMessage = true;
        }
    }

    private async Task UploadAsync()
    {
        try
        {
            var db = new DbContext(AppConfig.ConnectionString);

            var applicantRepo = new ApplicantRepository(db);
            var docRepo = new ApplicantDocumentRepository(db);

            var applicant =
                await applicantRepo.GetByAccountIdAsync(SessionManager.CurrentUserId ?? "");

            if (applicant == null)
            {
                StatusMessage = "Applicant not found.";
                HasMessage = true;
                return;
            }

            var requirementId = SelectedRequirementType?.Id?.Trim();

            // SAFE GUARD (prevents FK + length + invalid values)
            if (string.IsNullOrWhiteSpace(requirementId) || requirementId.Length != 36)
            {
                StatusMessage = "Invalid document type selected.";
                HasMessage = true;
                return;
            }

            await docRepo.CreateAsync(new ApplicantDocument
            {
                ApplicantId = applicant.ApplicantId,

                RequirementTypeId = requirementId,

                FilePath = FilePath,

                Status = DocumentStatus.Submitted,

                SubmittedAt = DateTime.Now
            });

            StatusMessage = "Upload successful.";
            HasMessage = true;

            FilePath = "";
            SelectedRequirementType = null;

            await LoadDocumentsAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            HasMessage = true;
        }
    }

    [RelayCommand]
    private void OpenDocument()
    {
        try
        {
            if (SelectedDocument == null)
                return;

            if (!File.Exists(SelectedDocument.FilePath))
            {
                StatusMessage = "File not found.";
                HasMessage = true;
                return;
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = SelectedDocument.FilePath,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            HasMessage = true;
        }
    }

    [RelayCommand]
    private async Task DeleteSelectedDocumentAsync()
    {
        try
        {
            if (SelectedDocument == null)
                return;

            var db = new DbContext(AppConfig.ConnectionString);
            var repo = new ApplicantDocumentRepository(db);

            await repo.DeleteAsync(SelectedDocument.DocumentId);

            await LoadDocumentsAsync();

            StatusMessage = "Document deleted.";
            HasMessage = true;
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            HasMessage = true;
        }
    }

    [RelayCommand]
    private void GoBack()
    {
        _mainViewModel?.NavigateToApplicantDashboard();
    }

    public class RequirementTypeOption
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
    }
}