using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRApplicantSystem.Data;
using HRApplicantSystem.Data.Models;
using HRApplicantSystem.Data.Repositories;
using HRApplicantSystem.Shared.Helpers;

namespace HRApplicantSystem.UI.ViewModels.HR;

/// <summary>
/// ViewModel for HR System Maintenance module.
/// Handles CRUD operations for departments and positions.
/// </summary>
public partial class MaintenanceViewModel : ViewModelBase
{
    private readonly MainWindowViewModel? _mainViewModel;

    /// <summary>
    /// Collection of all departments in the system.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<Department> departments = new();

    /// <summary>
    /// Collection of all positions in the system.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<Position> positions = new();

    /// <summary>
    /// Currently selected department.
    /// </summary>
    [ObservableProperty]
    private Department? selectedDepartment;

    /// <summary>
    /// Currently selected position.
    /// </summary>
    [ObservableProperty]
    private Position? selectedPosition;

    /// <summary>
    /// Department name for adding/editing.
    /// </summary>
    [ObservableProperty]
    private string departmentName = "";

    /// <summary>
    /// Department description for adding/editing.
    /// </summary>
    [ObservableProperty]
    private string departmentDescription = "";

    /// <summary>
    /// Position title for adding/editing.
    /// </summary>
    [ObservableProperty]
    private string positionTitle = "";

    /// <summary>
    /// Position description for adding/editing.
    /// </summary>
    [ObservableProperty]
    private string positionDescription = "";

    /// <summary>
    /// Status or error message to display.
    /// </summary>
    [ObservableProperty]
    private string message = "";

    /// <summary>
    /// True when a message should be displayed.
    /// </summary>
    [ObservableProperty]
    private bool hasMessage = false;

    /// <summary>
    /// True while data is loading.
    /// </summary>
    [ObservableProperty]
    private bool isLoading = false;

    public MaintenanceViewModel()
    {
        Debug.WriteLine("[MaintenanceViewModel] Initializing without MainWindowViewModel");
        _ = InitializeAsync();
    }

    public MaintenanceViewModel(MainWindowViewModel mainViewModel)
    {
        Debug.WriteLine("[MaintenanceViewModel] Initializing with MainWindowViewModel");
        _mainViewModel = mainViewModel;
        _ = InitializeAsync();
    }

    /// <summary>
    /// Initializes the ViewModel and loads data on construction.
    /// </summary>
    private async Task InitializeAsync()
    {
        Debug.WriteLine("[MaintenanceViewModel] InitializeAsync called");
        try
        {
            await LoadDepartmentsAsync();
            await LoadPositionsAsync();
            Message = "Maintenance module initialized.";
            HasMessage = true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[MaintenanceViewModel] InitializeAsync error: {ex.Message}");
            Message = "Failed to initialize maintenance module.";
            HasMessage = true;
        }
    }

    /// <summary>
    /// Loads all departments from the database.
    /// </summary>
    [RelayCommand]
    public async Task LoadDepartmentsAsync()
    {
        Debug.WriteLine("[MaintenanceViewModel] LoadDepartmentsAsync called");
        try
        {
            IsLoading = true;
            Departments.Clear();

            var db = new DbContext(AppConfig.ConnectionString);
            var deptRepo = new DepartmentRepository(db);
            var depts = await deptRepo.GetAllAsync();

            foreach (var dept in depts)
            {
                Departments.Add(dept);
            }

            Debug.WriteLine($"[MaintenanceViewModel] Loaded {Departments.Count} departments");
        }
        catch (Exception ex)
        {
            Message = $"Error loading departments: {ex.Message}";
            HasMessage = true;
            Debug.WriteLine($"[MaintenanceViewModel] LoadDepartmentsAsync error: {ex}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Loads all positions from the database.
    /// </summary>
    [RelayCommand]
    public async Task LoadPositionsAsync()
    {
        Debug.WriteLine("[MaintenanceViewModel] LoadPositionsAsync called");
        try
        {
            IsLoading = true;
            Positions.Clear();

            var db = new DbContext(AppConfig.ConnectionString);
            var posRepo = new PositionRepository(db);
            var positions = await posRepo.GetAllAsync();

            foreach (var pos in positions)
            {
                Positions.Add(pos);
            }

            Debug.WriteLine($"[MaintenanceViewModel] Loaded {Positions.Count} positions");
        }
        catch (Exception ex)
        {
            Message = $"Error loading positions: {ex.Message}";
            HasMessage = true;
            Debug.WriteLine($"[MaintenanceViewModel] LoadPositionsAsync error: {ex}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Adds a new department to the system.
    /// </summary>
    [RelayCommand]
    public async Task AddDepartmentAsync()
    {
        Debug.WriteLine("[MaintenanceViewModel] AddDepartmentAsync called");
        try
        {
            if (string.IsNullOrWhiteSpace(DepartmentName))
            {
                Message = "Please enter a department name.";
                HasMessage = true;
                return;
            }

            var db = new DbContext(AppConfig.ConnectionString);
            var deptRepo = new DepartmentRepository(db);

            var newDept = new Department
            {
                DepartmentId = Guid.NewGuid().ToString(),
                DepartmentName = DepartmentName,
                Description = DepartmentDescription,
                CreatedAt = DateTime.Now
            };

            bool success = await deptRepo.CreateAsync(newDept);

            if (success)
            {
                Departments.Add(newDept);
                DepartmentName = "";
                DepartmentDescription = "";
                Message = "Department added successfully!";
                Debug.WriteLine($"[MaintenanceViewModel] Department added: {newDept.DepartmentId}");
            }
            else
            {
                Message = "Failed to add department.";
            }

            HasMessage = true;
        }
        catch (Exception ex)
        {
            Message = $"Error adding department: {ex.Message}";
            HasMessage = true;
            Debug.WriteLine($"[MaintenanceViewModel] AddDepartmentAsync error: {ex}");
        }
    }

    /// <summary>
    /// Updates the selected department.
    /// </summary>
    [RelayCommand]
    public async Task EditDepartmentAsync()
    {
        Debug.WriteLine("[MaintenanceViewModel] EditDepartmentAsync called");
        try
        {
            if (SelectedDepartment == null)
            {
                Message = "Please select a department to edit.";
                HasMessage = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(DepartmentName))
            {
                Message = "Please enter a department name.";
                HasMessage = true;
                return;
            }

            var db = new DbContext(AppConfig.ConnectionString);
            var deptRepo = new DepartmentRepository(db);

            SelectedDepartment.DepartmentName = DepartmentName;
            SelectedDepartment.Description = DepartmentDescription;
            SelectedDepartment.UpdatedAt = DateTime.Now;

            bool success = await deptRepo.UpdateAsync(SelectedDepartment);

            if (success)
            {
                DepartmentName = "";
                DepartmentDescription = "";
                SelectedDepartment = null;
                Message = "Department updated successfully!";
                Debug.WriteLine("[MaintenanceViewModel] Department updated successfully");
                await LoadDepartmentsAsync();
            }
            else
            {
                Message = "Failed to update department.";
            }

            HasMessage = true;
        }
        catch (Exception ex)
        {
            Message = $"Error updating department: {ex.Message}";
            HasMessage = true;
            Debug.WriteLine($"[MaintenanceViewModel] EditDepartmentAsync error: {ex}");
        }
    }

    /// <summary>
    /// Deletes the selected department.
    /// </summary>
    [RelayCommand]
    public async Task DeleteDepartmentAsync()
    {
        Debug.WriteLine("[MaintenanceViewModel] DeleteDepartmentAsync called");
        try
        {
            if (SelectedDepartment == null)
            {
                Message = "Please select a department to delete.";
                HasMessage = true;
                return;
            }

            var db = new DbContext(AppConfig.ConnectionString);
            var deptRepo = new DepartmentRepository(db);

            bool success = await deptRepo.DeleteAsync(SelectedDepartment.DepartmentId);

            if (success)
            {
                Departments.Remove(SelectedDepartment);
                SelectedDepartment = null;
                Message = "Department deleted successfully!";
                Debug.WriteLine("[MaintenanceViewModel] Department deleted successfully");
            }
            else
            {
                Message = "Failed to delete department.";
            }

            HasMessage = true;
        }
        catch (Exception ex)
        {
            Message = $"Error deleting department: {ex.Message}";
            HasMessage = true;
            Debug.WriteLine($"[MaintenanceViewModel] DeleteDepartmentAsync error: {ex}");
        }
    }

    /// <summary>
    /// Adds a new position to the system.
    /// </summary>
    [RelayCommand]
    public async Task AddPositionAsync()
    {
        Debug.WriteLine("[MaintenanceViewModel] AddPositionAsync called");
        try
        {
            if (string.IsNullOrWhiteSpace(PositionTitle))
            {
                Message = "Please enter a position title.";
                HasMessage = true;
                return;
            }

            var db = new DbContext(AppConfig.ConnectionString);
            var posRepo = new PositionRepository(db);

            var newPos = new Position
            {
                PositionId = Guid.NewGuid().ToString(),
                PositionTitle = PositionTitle,
                Description = PositionDescription,
                CreatedAt = DateTime.Now
            };

            bool success = await posRepo.CreateAsync(newPos);

            if (success)
            {
                Positions.Add(newPos);
                PositionTitle = "";
                PositionDescription = "";
                Message = "Position added successfully!";
                Debug.WriteLine($"[MaintenanceViewModel] Position added: {newPos.PositionId}");
            }
            else
            {
                Message = "Failed to add position.";
            }

            HasMessage = true;
        }
        catch (Exception ex)
        {
            Message = $"Error adding position: {ex.Message}";
            HasMessage = true;
            Debug.WriteLine($"[MaintenanceViewModel] AddPositionAsync error: {ex}");
        }
    }

    /// <summary>
    /// Updates the selected position.
    /// </summary>
    [RelayCommand]
    public async Task EditPositionAsync()
    {
        Debug.WriteLine("[MaintenanceViewModel] EditPositionAsync called");
        try
        {
            if (SelectedPosition == null)
            {
                Message = "Please select a position to edit.";
                HasMessage = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(PositionTitle))
            {
                Message = "Please enter a position title.";
                HasMessage = true;
                return;
            }

            var db = new DbContext(AppConfig.ConnectionString);
            var posRepo = new PositionRepository(db);

            SelectedPosition.PositionTitle = PositionTitle;
            SelectedPosition.Description = PositionDescription;
            SelectedPosition.UpdatedAt = DateTime.Now;

            bool success = await posRepo.UpdateAsync(SelectedPosition);

            if (success)
            {
                PositionTitle = "";
                PositionDescription = "";
                SelectedPosition = null;
                Message = "Position updated successfully!";
                Debug.WriteLine("[MaintenanceViewModel] Position updated successfully");
                await LoadPositionsAsync();
            }
            else
            {
                Message = "Failed to update position.";
            }

            HasMessage = true;
        }
        catch (Exception ex)
        {
            Message = $"Error updating position: {ex.Message}";
            HasMessage = true;
            Debug.WriteLine($"[MaintenanceViewModel] EditPositionAsync error: {ex}");
        }
    }

    /// <summary>
    /// Deletes the selected position.
    /// </summary>
    [RelayCommand]
    public async Task DeletePositionAsync()
    {
        Debug.WriteLine("[MaintenanceViewModel] DeletePositionAsync called");
        try
        {
            if (SelectedPosition == null)
            {
                Message = "Please select a position to delete.";
                HasMessage = true;
                return;
            }

            var db = new DbContext(AppConfig.ConnectionString);
            var posRepo = new PositionRepository(db);

            bool success = await posRepo.DeleteAsync(SelectedPosition.PositionId);

            if (success)
            {
                Positions.Remove(SelectedPosition);
                SelectedPosition = null;
                Message = "Position deleted successfully!";
                Debug.WriteLine("[MaintenanceViewModel] Position deleted successfully");
            }
            else
            {
                Message = "Failed to delete position.";
            }

            HasMessage = true;
        }
        catch (Exception ex)
        {
            Message = $"Error deleting position: {ex.Message}";
            HasMessage = true;
            Debug.WriteLine($"[MaintenanceViewModel] DeletePositionAsync error: {ex}");
        }
    }

    /// <summary>
    /// Navigates back to the HR dashboard.
    /// </summary>
    [RelayCommand]
    public void GoBack()
    {
        Debug.WriteLine("[MaintenanceViewModel] GoBack called");
        _mainViewModel?.NavigateToHRDashboard();
    }
}
