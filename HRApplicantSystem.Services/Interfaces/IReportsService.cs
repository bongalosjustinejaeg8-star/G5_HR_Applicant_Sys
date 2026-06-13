namespace HRApplicantSystem.Services.Interfaces;

public interface IReportsService
{
    Task<dynamic?> GetApplicationStatisticsAsync(DateTime startDate, DateTime endDate);
    Task<dynamic?> GetApplicantsByStatusAsync();
    Task<dynamic?> GetApplicantsByVacancyAsync();
    Task<dynamic?> GetHiringTrendAnalyticsAsync();
    Task<byte[]?> ExportApplicationsReportAsync(string format);
}
