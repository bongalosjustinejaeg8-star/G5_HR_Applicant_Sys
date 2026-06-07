using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace HRApplicantSystem.UI.ViewModels
{
    public partial class DocumentsViewModel : ViewModelBase
    {
        [ObservableProperty] private string _resumePath = "No file chosen (e.g. Resume.pdf)";
        [ObservableProperty] private string _diplomaPath = "No file chosen (e.g. Transcript.pdf)";
        [ObservableProperty] private string _uploadStatus = "Ready to upload credentials.";

        [RelayCommand]
        private async Task UploadResume()
        {
            var path = await SelectFileAsync();
            if (!string.IsNullOrEmpty(path))
            {
                ResumePath = path;
                UploadStatus = "Resume file attached successfully!";
            }
        }

        [RelayCommand]
        private async Task UploadDiploma()
        {
            var path = await SelectFileAsync();
            if (!string.IsNullOrEmpty(path))
            {
                DiplomaPath = path;
                UploadStatus = "Academic file attached successfully!";
            }
        }

        private async Task<string?> SelectFileAsync()
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var topLevel = TopLevel.GetTopLevel(desktop.MainWindow);
                if (topLevel != null)
                {
                    var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                    {
                        Title = "Select Requirement Document",
                        AllowMultiple = false
                    });

                    if (files.Count > 0)
                    {
                        return files[0].Name;
                    }
                }
            }
            return null;
        }
    }
}