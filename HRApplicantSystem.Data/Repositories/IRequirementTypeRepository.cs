using HRApplicantSystem.Data.Models;

namespace HRApplicantSystem.Data.Repositories;

public interface IRequirementTypeRepository
{
    Task<IEnumerable<RequirementType>> GetAllAsync();
}