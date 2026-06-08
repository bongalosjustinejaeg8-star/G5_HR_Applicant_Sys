using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using HRApplicantSystem.Shared.Helpers;

namespace HRApplicantSystem.UI.ViewModels;

public class ApplicantListViewModel : ViewModelBase
{
    public ObservableCollection<dynamic> Applicants { get; set; } = new();

    private string _searchText = "";
    public string SearchText
    {
        get => _searchText;
        set { _searchText = value; OnPropertyChanged(); }
    }

    private string? _selectedStatus;
    public string? SelectedStatus
    {
        get => _selectedStatus;
        set { _selectedStatus = value; OnPropertyChanged(); }
    }

    public ApplicantListViewModel()
    {
        // TODO: Initialize applicant list
    }

    public async Task LoadApplicantsAsync()
    {
        // TODO: Load applicants from service
    }

    public async Task SearchApplicantsAsync()
    {
        // TODO: Implement search logic
    }
}
