using HRApplicantSystem.Data.Models;

namespace HRApplicantSystem.Services.Interfaces;

public interface IDashboardService
{
    Task<dynamic?> GetApplicantDashboardDataAsync(string applicantId);
    Task<dynamic?> GetHRDashboardDataAsync();
}
