using System.Collections.ObjectModel;
using HRApplicantSystem.Shared.Helpers;

namespace HRApplicantSystem.UI.ViewModels;

public class JobVacancyMgmtViewModel : ViewModelBase
{
    public ObservableCollection<dynamic> JobVacancies { get; set; } = new();

    private dynamic? _selectedJobVacancy;
    public dynamic? SelectedJobVacancy
    {
        get => _selectedJobVacancy;
        set { _selectedJobVacancy = value; OnPropertyChanged(); }
    }

    public JobVacancyMgmtViewModel()
    {
        // TODO: Initialize job vacancy management
    }

    public async Task LoadJobVacanciesAsync()
    {
        // TODO: Load job vacancies from service
    }

    public async Task AddJobVacancyAsync()
    {
        // TODO: Implement add job vacancy logic
    }

    public async Task EditJobVacancyAsync()
    {
        // TODO: Implement edit job vacancy logic
    }

    public async Task CloseJobVacancyAsync()
    {
        // TODO: Implement close job vacancy logic
    }
}
