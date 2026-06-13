using HRApplicantSystem.Data.Models;

namespace HRApplicantSystem.Services.Interfaces;

public interface IMaintenanceService
{
    Task<IEnumerable<Department>> GetAllDepartmentsAsync();
    Task<bool> CreateDepartmentAsync(Department department);
    Task<bool> UpdateDepartmentAsync(Department department);
    Task<bool> DeleteDepartmentAsync(string departmentId);
    Task<bool> DeletePositionAsync(string positionId);
}
