using HRApplicantSystem.Data.Models;

namespace HRApplicantSystem.Data.Repositories;

public interface IRoleRepository
{
    Task<IEnumerable<Role>> GetAllAsync();
    Task<Role?> GetByIdAsync(string id);
    Task<bool> CreateAsync(Role role);
    Task<bool> DeleteAsync(string id);
}