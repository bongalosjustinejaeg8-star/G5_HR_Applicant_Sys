using HRApplicantSystem.Data.Models;

namespace HRApplicantSystem.Data.Repositories;

public interface IScreeningResultRepository
{
    Task<ScreeningResult?> GetByApplicationIdAsync(string applicationId);
    Task<bool> CreateAsync(ScreeningResult result);
    Task<bool> UpdateAsync(ScreeningResult result);
}
