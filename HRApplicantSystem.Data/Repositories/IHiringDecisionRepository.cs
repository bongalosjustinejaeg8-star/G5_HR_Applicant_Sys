using HRApplicantSystem.Data.Models;

namespace HRApplicantSystem.Data.Repositories;

public interface IHiringDecisionRepository
{
    Task<HiringDecision?> GetByApplicationIdAsync(string applicationId);
    Task<IEnumerable<HiringDecision>> GetAllAsync();
    Task<bool> CreateAsync(HiringDecision decision);
    Task<bool> UpdateAsync(HiringDecision decision);
}
