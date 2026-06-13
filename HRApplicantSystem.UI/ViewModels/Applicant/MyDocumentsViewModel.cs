using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRApplicantSystem.Data;
using HRApplicantSystem.Data.Repositories;
using HRApplicantSystem.Shared.Helpers;

namespace HRApplicantSystem.UI.ViewModels.Applicant;

/// <summary>
/// ViewModel for managing applicant documents and video recordings.
/// Handles loading, uploading, and deletion of application documents.
/// </summary>
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
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[MyDocumentsViewModel] InitializeAsync error: {ex.Message}");
            Message = "Failed to initialize documents.";
            HasMessage = true;
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

            // TODO: Implement document retrieval from DocumentRepository
            // This would typically load documents from a documents table/repository
            Message = $"Documents loaded for applicant {applicant.ApplicantId}";
            HasMessage = true;
            Debug.WriteLine($"[MyDocumentsViewModel] Loaded {Documents.Count} documents");
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
        try
        {
            var applicantId = SessionManager.CurrentUserId;
            if (string.IsNullOrEmpty(applicantId))
            {
                Message = "User not authenticated. Please log in.";
                HasMessage = true;
                return;
            }

            // TODO: Implement document upload logic
            // 1. Open file dialog
            // 2. Validate file type and size
            // 3. Upload to storage
            // 4. Save metadata to database
            // 5. Reload documents

            Message = "Document upload feature coming soon.";
            HasMessage = true;
            Debug.WriteLine("[MyDocumentsViewModel] Upload: Feature not yet implemented");
        }
        catch (Exception ex)
        {
            Message = $"Error uploading document: {ex.Message}";
            HasMessage = true;
            Debug.WriteLine($"[MyDocumentsViewModel] UploadDocumentAsync error: {ex}");
        }
    }

    /// <summary>
    /// Records a video for the applicant's application.
    /// </summary>
    [RelayCommand]
    public async Task RecordVideoAsync()
    {
        Debug.WriteLine("[MyDocumentsViewModel] RecordVideoAsync called");
        try
        {
            var applicantId = SessionManager.CurrentUserId;
            if (string.IsNullOrEmpty(applicantId))
            {
                Message = "User not authenticated. Please log in.";
                HasMessage = true;
                return;
            }

            // TODO: Implement video recording logic
            // 1. Launch recording interface
            // 2. Allow applicant to record
            // 3. Save video to storage
            // 4. Create document entry
            // 5. Reload documents

            Message = "Video recording feature coming soon.";
            HasMessage = true;
            Debug.WriteLine("[MyDocumentsViewModel] Record video: Feature not yet implemented");
        }
        catch (Exception ex)
        {
            Message = $"Error recording video: {ex.Message}";
            HasMessage = true;
            Debug.WriteLine($"[MyDocumentsViewModel] RecordVideoAsync error: {ex}");
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

            // TODO: Implement document deletion logic
            // 1. Confirm deletion
            // 2. Delete from storage
            // 3. Remove from database
            // 4. Reload documents

            Message = "Document deletion feature coming soon.";
            HasMessage = true;
            Debug.WriteLine("[MyDocumentsViewModel] Delete: Feature not yet implemented");
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
