using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using HRApplicantSystem.Shared.Helpers;

namespace HRApplicantSystem.UI.ViewModels;

public class MyDocumentsViewModel : ViewModelBase
{
    public ObservableCollection<dynamic> Documents { get; set; } = new();

    public MyDocumentsViewModel()
    {
        // TODO: Initialize my documents
    }

    public async Task LoadDocumentsAsync()
    {
        // TODO: Load applicant documents
    }

    public async Task UploadDocumentAsync()
    {
        // TODO: Implement document upload
    }

    public async Task RecordVideoAsync()
    {
        // TODO: Implement video recording
    }

    public async Task DeleteDocumentAsync()
    {
        // TODO: Implement document deletion
    }
}
