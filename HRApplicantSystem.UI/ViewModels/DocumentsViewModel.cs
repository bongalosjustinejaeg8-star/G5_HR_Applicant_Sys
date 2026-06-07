using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using HRApplicantSystem.UI.ViewModels;

namespace HRApplicantSystem.UI.ViewModels
{
    public partial class DocumentsViewModel : ViewModelBase
    {
        // 🌟 Tugma sa kung ano ang binabasa ng AXAML mo ngayon
        [ObservableProperty] private string _resumeStatus = "Not Uploaded";
        [ObservableProperty] private string _torStatus = "Not Uploaded";
        [ObservableProperty] private string _idStatus = "Not Uploaded";
        [ObservableProperty] private string _certificateStatus = "Not Uploaded";
        [ObservableProperty] private string _documentRemarks = "No remarks yet.";

        [RelayCommand]
        private async Task UploadResume()
        {
            var topLevel = TopLevel.GetTopLevel(Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null);
            if (topLevel != null)
            {
                var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                {
                    Title = "Open Resume Document",
                    AllowMultiple = false,
                    FileTypeFilter = new[] { FilePickerFileTypes.Pdf, FilePickerFileTypes.All }
                });
                if (files.Count > 0)
                {
                    ResumeStatus = $"Uploaded: {files[0].Name}";
                    DocumentRemarks = "Resume updated successfully!";
                }
            }
        }

        [RelayCommand]
        private async Task UploadID()
        {
            var topLevel = TopLevel.GetTopLevel(Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null);
            if (topLevel != null)
            {
                var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                {
                    Title = "Open Valid Government ID",
                    AllowMultiple = false,
                    FileTypeFilter = new[] { FilePickerFileTypes.ImagePng, FilePickerFileTypes.ImageJpg, FilePickerFileTypes.Pdf }
                });
                if (files.Count > 0)
                {
                    IdStatus = $"Uploaded: {files[0].Name}";
                    DocumentRemarks = "Valid ID updated successfully!";
                }
            }
        }

        [RelayCommand]
        private async Task UploadTOR()
        {
            var topLevel = TopLevel.GetTopLevel(Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null);
            if (topLevel != null)
            {
                var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                {
                    Title = "Open Transcript of Records",
                    AllowMultiple = false,
                    FileTypeFilter = new[] { FilePickerFileTypes.Pdf, FilePickerFileTypes.All }
                });
                if (files.Count > 0)
                {
                    TorStatus = $"Uploaded: {files[0].Name}";
                    DocumentRemarks = "TOR updated successfully!";
                }
            }
        }

        [RelayCommand]
        private async Task UploadCertificate()
        {
            var topLevel = TopLevel.GetTopLevel(Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null);
            if (topLevel != null)
            {
                var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                {
                    Title = "Open Certificate Document",
                    AllowMultiple = false,
                    FileTypeFilter = new[] { FilePickerFileTypes.Pdf, FilePickerFileTypes.All }
                });
                if (files.Count > 0)
                {
                    CertificateStatus = $"Uploaded: {files[0].Name}";
                    DocumentRemarks = "Certificate updated successfully!";
                }
            }
        }
    }
}