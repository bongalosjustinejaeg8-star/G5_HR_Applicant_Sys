using System.Collections.ObjectModel;
using HRApplicantSystem.Shared.Helpers;

namespace HRApplicantSystem.UI.ViewModels;

public class JobVacanciesViewModel : ViewModelBase
{
    public ObservableCollection<dynamic> JobVacancies { get; set; } = new();

    private dynamic? _selectedJobVacancy;
    public dynamic? SelectedJobVacancy
    {
        get => _selectedJobVacancy;
        set { _selectedJobVacancy = value; OnPropertyChanged(); }
    }

    private string _searchText = "";
    public string SearchText
    {
        get => _searchText;
        set { _searchText = value; OnPropertyChanged(); }
    }

    private string? _selectedDepartmentFilter;
    public string? SelectedDepartmentFilter
    {
        get => _selectedDepartmentFilter;
        set { _selectedDepartmentFilter = value; OnPropertyChanged(); }
    }

    public JobVacanciesViewModel()
    {
        // TODO: Initialize job vacancies
    }

    public async Task LoadJobVacanciesAsync()
    {
        // TODO: Load job vacancies
    }

    public async Task ApplyForJobAsync()
    {
        // TODO: Implement job application
    }
}
