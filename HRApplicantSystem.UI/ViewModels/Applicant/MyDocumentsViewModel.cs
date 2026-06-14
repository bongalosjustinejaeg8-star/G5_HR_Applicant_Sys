using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
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

    public ObservableCollection<ApplicantDocument> Documents { get; set; } = new();

    [ObservableProperty] private string _documentType = string.Empty;
    [ObservableProperty] private string _filePath = string.Empty;
    [ObservableProperty] private string _statusMessage = string.Empty;
    [ObservableProperty] private bool _hasMessage = false;

    public MyDocumentsViewModel() { _ = LoadDocumentsAsync(); }
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
            var applicant = await applicantRepo.GetByAccountIdAsync(SessionManager.CurrentUserId ?? string.Empty);
            if (applicant == null) return;
            var docs = await docRepo.GetByApplicantIdAsync(applicant.ApplicantId);
            Documents.Clear();
            foreach (var d in docs) Documents.Add(d);
        }
        catch (Exception ex) { StatusMessage = ex.Message; HasMessage = true; }
    }

    [RelayCommand]
    public async Task UploadDocumentAsync()
    {
        if (string.IsNullOrWhiteSpace(DocumentType) || string.IsNullOrWhiteSpace(FilePath))
        {
            StatusMessage = "Please enter document type and file path.";
            HasMessage = true;
            return;
        }
        try
        {
            var db = new DbContext(AppConfig.ConnectionString);
            var applicantRepo = new ApplicantRepository(db);
            var docRepo = new ApplicantDocumentRepository(db);
            var applicant = await applicantRepo.GetByAccountIdAsync(SessionManager.CurrentUserId ?? string.Empty);
            if (applicant == null) { StatusMessage = "Profile not found."; HasMessage = true; return; }
            var doc = new ApplicantDocument
            {
                ApplicantId = applicant.ApplicantId,
                RequirementTypeId = DocumentType,
                FilePath = FilePath,
                Status = DocumentStatus.Submitted
            };
            await docRepo.CreateAsync(doc);
            StatusMessage = "Document submitted successfully!";
            HasMessage = true;
            DocumentType = string.Empty;
            FilePath = string.Empty;
            await LoadDocumentsAsync();
        }
        catch (Exception ex) { StatusMessage = ex.Message; HasMessage = true; }
    }

    [RelayCommand]
    public async Task DeleteDocumentAsync(string documentId)
    {
        try
        {
            var db = new DbContext(AppConfig.ConnectionString);
            var docRepo = new ApplicantDocumentRepository(db);
            await docRepo.DeleteAsync(documentId);
            await LoadDocumentsAsync();
        }
        catch (Exception ex) { StatusMessage = ex.Message; HasMessage = true; }
    }

    [RelayCommand] private void GoBack() => _mainViewModel?.NavigateToApplicantDashboard();
}