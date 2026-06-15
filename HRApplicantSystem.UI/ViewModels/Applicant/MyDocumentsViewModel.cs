using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;

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

    public ObservableCollection<ApplicantDocument> Documents { get; }
        = new();

    [ObservableProperty]
    private ApplicantDocument? selectedDocument;

    [ObservableProperty]
    private string documentType = "";

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

    public MyDocumentsViewModel(
        MainWindowViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;

        _ = LoadDocumentsAsync();
    }

    public async Task LoadDocumentsAsync()
    {
        try
        {
            var db =
                new DbContext(
                    AppConfig.ConnectionString);

            var applicantRepo =
                new ApplicantRepository(db);

            var docRepo =
                new ApplicantDocumentRepository(db);

            var applicant =
                await applicantRepo.GetByAccountIdAsync(
                    SessionManager.CurrentUserId ?? "");

            if (applicant == null)
                return;

            var docs =
                await docRepo.GetByApplicantIdAsync(
                    applicant.ApplicantId);

            Documents.Clear();

            foreach (var doc in docs)
                Documents.Add(doc);
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
            if (Avalonia.Application.Current?.ApplicationLifetime
                is not IClassicDesktopStyleApplicationLifetime desktop)
            {
                StatusMessage = "Window unavailable.";
                HasMessage = true;
                return;
            }

            var files =
                await desktop.MainWindow!
                    .StorageProvider
                    .OpenFilePickerAsync(
                        new FilePickerOpenOptions
                        {
                            Title = "Upload Document",
                            AllowMultiple = false
                        });

            if (files.Count == 0)
                return;

            FilePath =
                files[0]
                .Path
                .LocalPath;

            await UploadDocumentAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            HasMessage = true;
        }
    }

    public async Task UploadDocumentAsync()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(FilePath))
            {
                StatusMessage =
                    "Please select a file.";

                HasMessage = true;

                return;
            }

            if (!File.Exists(FilePath))
            {
                StatusMessage =
                    "File not found.";

                HasMessage = true;

                return;
            }

            var ext =
                Path
                .GetExtension(FilePath)
                .ToLower();

            if (ext == ".zip")
            {
                StatusMessage =
                    "ZIP files are not allowed.";

                HasMessage = true;

                return;
            }

            var db =
                new DbContext(
                    AppConfig.ConnectionString);

            var applicantRepo =
                new ApplicantRepository(db);

            var docRepo =
                new ApplicantDocumentRepository(db);

            var applicant =
                await applicantRepo
                    .GetByAccountIdAsync(
                        SessionManager.CurrentUserId ?? "");

            if (applicant == null)
            {
                StatusMessage =
                    "Applicant not found.";

                HasMessage = true;

                return;
            }

            await docRepo.CreateAsync(
                new ApplicantDocument
                {
                    ApplicantId =
                        applicant.ApplicantId,

                    RequirementTypeId =
                        DocumentType,

                    FilePath =
                        FilePath,

                    Status =
                        DocumentStatus.Submitted
                });

            StatusMessage =
                "Document uploaded.";

            HasMessage = true;

            FilePath = "";

            await LoadDocumentsAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;

            HasMessage = true;
        }
        if (string.IsNullOrWhiteSpace(DocumentType))
        {
            StatusMessage =
                "Document type required.";

            HasMessage = true;

            return;
        }
    }

    [RelayCommand]
    public void OpenDocument()
    {
        try
        {
            if (SelectedDocument == null)
                return;

            if (!File.Exists(
                SelectedDocument.FilePath))
                return;

            Process.Start(
                new ProcessStartInfo
                {
                    FileName =
                        SelectedDocument.FilePath,

                    UseShellExecute =
                        true
                });
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;

            HasMessage = true;
        }
    }

    [RelayCommand]
    public async Task DeleteSelectedDocumentAsync()
    {
        try
        {
            if (SelectedDocument == null)
                return;

            var db =
                new DbContext(
                    AppConfig.ConnectionString);

            var repo =
                new ApplicantDocumentRepository(
                    db);

            await repo.DeleteAsync(
                SelectedDocument.DocumentId);

            await LoadDocumentsAsync();

            StatusMessage =
                "Document deleted.";

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
        _mainViewModel
            ?.NavigateToApplicantDashboard();
    }
}