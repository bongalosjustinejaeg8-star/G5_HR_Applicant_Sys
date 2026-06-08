using HRApplicantSystem.Data.Models;

namespace HRApplicantSystem.Services.Interfaces;

public interface IMaintenanceService
{
    Task<IEnumerable<Department>> GetAllDepartmentsAsync();
    Task<bool> CreateDepartmentAsync(Department department);
    Task<bool> UpdateDepartmentAsync(Department department);
    Task<bool> DeleteDepartmentAsync(string departmentId);

    Task<IEnumerable<Position>> GetAllPositionsAsync();
    Task<bool> CreatePositionAsync(Position position);
    Task<bool> UpdatePositionAsync(Position position);
    Task<bool> DeletePositionAsync(string positionId);
}
