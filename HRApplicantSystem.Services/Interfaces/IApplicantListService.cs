using HRApplicantSystem.Data.Models;

namespace HRApplicantSystem.Services.Interfaces;

public interface IApplicantListService
{
    Task<IEnumerable<Application>> GetAllApplicationsAsync();
    Task<IEnumerable<Application>> SearchApplicationsAsync(string keyword);
    Task<IEnumerable<Application>> FilterApplicationsAsync(string status, string vacancyId);
    Task<Application?> GetApplicationDetailsAsync(string applicationId);
}
