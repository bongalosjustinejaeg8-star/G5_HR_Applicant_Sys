using System.Collections.ObjectModel;
using HRApplicantSystem.Shared.Helpers;

namespace HRApplicantSystem.UI.ViewModels;

public class MaintenanceViewModel : ViewModelBase
{
    public ObservableCollection<dynamic> Departments { get; set; } = new();
    public ObservableCollection<dynamic> Positions { get; set; } = new();

    public MaintenanceViewModel()
    {
        // TODO: Initialize maintenance view
    }

    public async Task LoadDepartmentsAsync()
    {
        // TODO: Load departments
    }

    public async Task LoadPositionsAsync()
    {
        // TODO: Load positions
    }

    public async Task AddDepartmentAsync()
    {
        // TODO: Implement add department
    }

    public async Task AddPositionAsync()
    {
        // TODO: Implement add position
    }
}
