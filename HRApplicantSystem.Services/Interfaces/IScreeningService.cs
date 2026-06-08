using HRApplicantSystem.Data.Models;
using HRApplicantSystem.Shared.Enums;

namespace HRApplicantSystem.Services.Interfaces;

public interface IScreeningService
{
    Task<IEnumerable<Application>> GetApplicationsForScreeningAsync();
    Task<string> SaveScreeningResultAsync(string applicationId, string hrUserId, ScreeningResults result, string remarks);
}