using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Avalonia.Platform.Storage;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRApplicantSystem.Data;
using HRApplicantSystem.Data.Repositories;
using HRApplicantSystem.Shared.Helpers;
using HRApplicantSystem.Data.Models;

namespace HRApplicantSystem.UI.ViewModels.Applicant;

public partial class MyDocumentsViewModel : ViewModelBase
{
    private readonly MainWindowViewModel? _mainViewModel;

    [ObservableProperty]
    private ObservableCollection<ApplicantDocument> documents = new();

    [ObservableProperty]
    private ApplicantDocument? selectedDocument;

    [ObservableProperty]
    private ObservableCollection<RequirementType> requirementTypes = new();

    [ObservableProperty]
    private RequirementType? selectedRequirementType;

    [ObservableProperty]
    private string message = "";

    [ObservableProperty]
    private bool hasMessage = false;

    [ObservableProperty]
    private bool isLoading = false;

    public MyDocumentsViewModel()
    {
        Debug.WriteLine("[MyDocumentsViewModel] Initializing without MainWindowViewModel");
        _ = InitializeAsync();
    }

    public MyDocumentsViewModel(MainWindowViewModel mainViewModel)
    {
        Debug.WriteLine("[MyDocumentsViewModel] Initializing with MainWindowViewModel");
        _mainViewModel = mainViewModel;
        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        Debug.WriteLine("[MyDocumentsViewModel] InitializeAsync called");
        try
        {
            await LoadDocumentsAsync();
            await LoadRequirementTypesAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[MyDocumentsViewModel] InitializeAsync error: {ex.Message}");
            Message = "Failed to initialize documents.";
            HasMessage = true;
        }
    }

    private async Task LoadRequirementTypesAsync()
    {
        Debug.WriteLine("[MyDocumentsViewModel] LoadRequirementTypesAsync called");
        try
        {
            var db = new DbContext(AppConfig.ConnectionString);
            var requirementTypeRepo = new RequirementTypeRepository(db);
            var types = await requirementTypeRepo.GetAllAsync();

            RequirementTypes = new ObservableCollection<RequirementType>(types);

            Debug.WriteLine($"[MyDocumentsViewModel] Loaded {RequirementTypes.Count} requirement types");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[MyDocumentsViewModel] LoadRequirementTypesAsync error: {ex.Message}");
        }   
    }

   [RelayCommand]
    public async Task LoadDocumentsAsync()
    {
        Debug.WriteLine(">>> DEBUG: LoadDocumentsAsync started <<<");
        try
        {
            IsLoading = true;
            Documents.Clear();

            var applicantId = SessionManager.CurrentUserId;
            if (string.IsNullOrEmpty(applicantId))
            {
                Message = " DEBUG: No CurrentUserID found (not logged in.)";
                HasMessage = true;
                return;
            }
        
            var db = new DbContext(AppConfig.ConnectionString);
            var applicantRepo = new ApplicantRepository(db);
            var applicant = await applicantRepo.GetByAccountIdAsync(applicantId);

            if (applicant == null)
            {
                Message = $" DEBUG: No applicant profile found for AccountId: {applicantId}";
                HasMessage = true;
                return;
            }

            var documentRepo = new ApplicantDocumentRepository(db);
            var applicantDocuments = await documentRepo.GetByApplicantIdAsync(applicant.ApplicantId);

            var docList = applicantDocuments.ToList();

            foreach (var document in docList)
            {
                Documents.Add(document);
            }

            Message = $"DEBUG: applicantId={applicant.ApplicantId}, found {docList.Count} docs, Documents.Count={Documents.Count}";
            HasMessage = true;
        }
        catch (Exception ex)
        {
            Message = $"DEBUG ERROR: {ex.Message}";
            HasMessage = true;  
            Debug.WriteLine($">>> DEBUG ERROR: {ex} <<<");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task UploadDocumentAsync()
    {
        Debug.WriteLine("[MyDocumentsViewModel] UploadDocumentAsync called");

        if (SelectedRequirementType == null)
        {
            Message = "Please select a document type first.";
            HasMessage = true;
            return;
        }

        var mainWindow = Avalonia.Application.Current?.ApplicationLifetime
            as Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime;
        var window = mainWindow?.MainWindow;
        if (window == null) return;

        var files = await window.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select Document or Video",
            AllowMultiple = false
        });

        if (files.Count == 0) return;

        var file = files[0];

        try
        {
            IsLoading = true;

            var applicantId = SessionManager.CurrentUserId;
            if (string.IsNullOrEmpty(applicantId))
            {
                Message = "User not authenticated. Please log in.";
                HasMessage = true;
                return;
            }

            var db = new DbContext(AppConfig.ConnectionString);
            var applicantRepo = new ApplicantRepository(db);
            var applicant = await applicantRepo.GetByAccountIdAsync(applicantId);

            if (applicant == null)
            {
                Message = "Applicant profile not found.";
                HasMessage = true;
                return;
            }

            var uploadsFolder = Path.Combine(
                AppContext.BaseDirectory, "Uploads", applicant.ApplicantId.ToString());

            Directory.CreateDirectory(uploadsFolder);

            var destFileName = $"{Guid.NewGuid()}_{file.Name}";
            var destPath = Path.Combine(uploadsFolder, destFileName);

            await using (var sourceStream = await file.OpenReadAsync())
            await using (var destStream = File.Create(destPath))
            {
                await sourceStream.CopyToAsync(destStream);
            }

            var documentRepo = new ApplicantDocumentRepository(db);

            var newDocument = new ApplicantDocument
            {
                ApplicantId = applicant.ApplicantId,
                RequirementTypeId = SelectedRequirementType.RequirementTypeId,
                FilePath = destPath,
                Status = HRApplicantSystem.Shared.Enums.DocumentStatus.Submitted 
            };

            await documentRepo.CreateAsync(newDocument);

            await LoadDocumentsAsync();

            Message = $"{file.Name} uploaded successfully as {SelectedRequirementType.Name}.";
            HasMessage = true;
        }
        catch (Exception ex)
        {
            Message = $"Error uploading file: {ex.Message}";
            HasMessage = true;
            Debug.WriteLine($"[MyDocumentsViewModel] UploadDocumentAsync error: {ex}");
        }
        finally
        {
            IsLoading = false;
        }
    }


    [RelayCommand]
    public void RecordVideo()
    {
        Debug.WriteLine("[MyDocumentsViewModel] RecordVideo called");
        try
        {
            var processInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "microsoft.windows.camera:",
                UseShellExecute = true
            };

            System.Diagnostics.Process.Start(processInfo);

            Message = "Recording started in Camera app. Please save the file then use 'Upload' to submit.";
            HasMessage = true;
        }
        catch (Exception ex)
        {
            Message = $"Error opening camera: {ex.Message}";
            HasMessage = true;
            Debug.WriteLine($"[MyDocumentsViewModel] RecordVideo error: {ex}");
        }
    }

   [RelayCommand]
    public async Task DownloadDocumentAsync()
    {
        Debug.WriteLine("[MyDocumentsViewModel] DownloadDocumentAsync called");
        try
        {
            if (SelectedDocument is not ApplicantDocument document)
            {
                Message = "Please select a document.";
                HasMessage = true;
                return;
            }

            if (!File.Exists(document.FilePath))
            {
                Message = "File not found.";
                HasMessage = true;
                return;
            }

            await Task.Run(() =>
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = document.FilePath,
                    UseShellExecute = true
                });
            });

        Message = "Document opened successfully.";
        HasMessage = true;
    }
    catch (Exception ex)
    {
        Message = $"Error opening document: {ex.Message}";
        HasMessage = true;
        Debug.WriteLine($"Download error: {ex}");

    }
}

    [RelayCommand]

    public async Task DeleteDocumentAsync()
    {
        Debug.WriteLine($"[DEBUG] Delete clicked. SelectedDocument is: {SelectedDocument}");
        try
        {
            if (SelectedDocument == null)
            {
                Message = "Please select a document to delete.";
                HasMessage = true;
                return;
            }

            var document = SelectedDocument as ApplicantDocument;

            if (document == null)
            {
                Message = "Invalid document selected.";
                HasMessage = true;
                return;
            }

            var db = new DbContext(AppConfig.ConnectionString);
            var documentRepo = new ApplicantDocumentRepository(db);

            var deleted = await documentRepo.DeleteAsync(document.DocumentId);

            if (deleted)
            {
                await LoadDocumentsAsync();
                Message = "Document deleted successfully.";
            }
            else
            {
                Message = "Failed to delete document.";
            }

            HasMessage = true;
        }
        catch (Exception ex)
        {
            Message = $"Error deleting document: {ex.Message}";
            HasMessage = true;
            Debug.WriteLine($"[MyDocumentsViewModel] DeleteDocumentAsync error: {ex}");
        }
    }


    [RelayCommand]
    public void GoBack()
    {
        Debug.WriteLine("[MyDocumentsViewModel] GoBack called");
        _mainViewModel?.NavigateToApplicantDashboard();
    }
}