using System;
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

    /// <summary>
    /// Collection of applicant documents.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<dynamic> documents = new();

    /// <summary>
    /// Currently selected document.
    /// </summary>
    [ObservableProperty]
    private object? selectedDocument;

    /// <summary>
    /// Available requirement types for document upload.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<RequirementType> requirementTypes = new();

    /// <summary>
    /// Currently selected requirement type for upload.
    /// </summary>
    [ObservableProperty]
    private RequirementType? selectedRequirementType;

    /// <summary>
    /// Status or message to display to the user.
    /// </summary>
    [ObservableProperty]
    private string message = "";

    /// <summary>
    /// True when a message should be displayed.
    /// </summary>
    [ObservableProperty]
    private bool hasMessage = false;

    /// <summary>
    /// True while documents are loading.
    /// </summary>
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

    /// <summary>
    /// Initializes the ViewModel and loads documents on construction.
    /// </summary>
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

    /// <summary>
    /// Loads available requirement types for the upload dropdown.
    /// </summary>
    private async Task LoadRequirementTypesAsync()
    {
        Debug.WriteLine("[MyDocumentsViewModel] LoadRequirementTypesAsync called");
        try
        {
            var db = new DbContext(AppConfig.ConnectionString);
            var requirementTypeRepo = new RequirementTypeRepository(db);
            var types = await requirementTypeRepo.GetAllAsync();

            RequirementTypes.Clear();
            foreach (var type in types)
            {
                RequirementTypes.Add(type);
            }

            Debug.WriteLine($"[MyDocumentsViewModel] Loaded {RequirementTypes.Count} requirement types");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[MyDocumentsViewModel] LoadRequirementTypesAsync error: {ex}");
        }
    }

    /// <summary>
    /// Loads all documents for the current applicant from the database.
    /// </summary>
    [RelayCommand]
    public async Task LoadDocumentsAsync()
    {
        Debug.WriteLine("[MyDocumentsViewModel] LoadDocumentsAsync called");
        try
        {
            IsLoading = true;
            Documents.Clear();

            var applicantId = SessionManager.CurrentUserId;
            if (string.IsNullOrEmpty(applicantId))
            {
                Message = "User not authenticated. Please log in.";
                HasMessage = true;
                Debug.WriteLine("[MyDocumentsViewModel] No current user ID");
                return;
            }

            var db = new DbContext(AppConfig.ConnectionString);
            var applicantRepo = new ApplicantRepository(db);
            var applicant = await applicantRepo.GetByAccountIdAsync(applicantId);

            if (applicant == null)
            {
                Message = "Applicant profile not found.";
                HasMessage = true;
                Debug.WriteLine("[MyDocumentsViewModel] Applicant not found for user");
                return;
            }

            var documentRepo = new ApplicantDocumentRepository(db);

            var applicantDocuments =
                await documentRepo.GetByApplicantIdAsync(applicant.ApplicantId);

            foreach (var document in applicantDocuments)
            {
                Documents.Add(document);
            }

            Message = $"{Documents.Count} document(s) loaded successfully.";
            HasMessage = true;

            Debug.WriteLine(
                $"[MyDocumentsViewModel] Loaded {Documents.Count} documents");
        }
        catch (Exception ex)
        {
            Message = $"Error loading documents: {ex.Message}";
            HasMessage = true;
            Debug.WriteLine($"[MyDocumentsViewModel] LoadDocumentsAsync error: {ex}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Uploads a new document for the applicant.
    /// </summary>
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

            // Save file to local storage
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

            // Create document entry in database
            var documentRepo = new ApplicantDocumentRepository(db);

            var newDocument = new ApplicantDocument
            {
                ApplicantId = applicant.ApplicantId,
                RequirementTypeId = SelectedRequirementType.RequirementTypeId,
                FilePath = destPath,
                Status = HRApplicantSystem.Shared.Enums.DocumentStatus.Submitted 
            };

            await documentRepo.CreateAsync(newDocument);

            // Reload documents
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

    /// <summary>
    /// Opens the camera app for video recording.
    /// </summary>
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

    /// <summary>
    /// Downloads the selected document to a chosen location.
    /// </summary>
    [RelayCommand]
    public async Task DownloadDocumentAsync()
    {
        Debug.WriteLine("[MyDocumentsViewModel] DownloadDocumentAsync called");
        if (SelectedDocument is not ApplicantDocument document)
        {
            Message = "Please select a document to download.";
            HasMessage = true;
            return;
        }

        if (string.IsNullOrEmpty(document.FilePath) || !File.Exists(document.FilePath))
        {
            Message = "File not found on disk.";
            HasMessage = true;
            return;
        }

        var mainWindow = Avalonia.Application.Current?.ApplicationLifetime
            as Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime;
        var window = mainWindow?.MainWindow;
        if (window == null) return;

        var suggestedName = Path.GetFileName(document.FilePath);

        var file = await window.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save Document",
            SuggestedFileName = suggestedName
        });

        if (file != null)
        {
            try
            {
                await using var sourceStream = File.OpenRead(document.FilePath);
                await using var destStream = await file.OpenWriteAsync();
                await sourceStream.CopyToAsync(destStream);

                Message = $"Saved to {file.Name}";
                HasMessage = true;
            }
            catch (Exception ex)
            {
                Message = $"Error saving file: {ex.Message}";
                HasMessage = true;
                Debug.WriteLine($"[MyDocumentsViewModel] DownloadDocumentAsync error: {ex}");
            }
        }
    }

    /// <summary>
    /// Deletes the selected document.
    /// </summary>
    [RelayCommand]
    public async Task DeleteDocumentAsync()
    {
        Debug.WriteLine("[MyDocumentsViewModel] DeleteDocumentAsync called");
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

    /// <summary>
    /// Navigates back to the applicant dashboard.
    /// </summary>
    [RelayCommand]
    public void GoBack()
    {
        Debug.WriteLine("[MyDocumentsViewModel] GoBack called");
        _mainViewModel?.NavigateToApplicantDashboard();
    }
}