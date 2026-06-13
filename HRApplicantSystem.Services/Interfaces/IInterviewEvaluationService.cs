using HRApplicantSystem.Data.Models;

namespace HRApplicantSystem.Services.Interfaces;

public interface IInterviewEvaluationService
{
    Task<IEnumerable<Application>> GetApplicationsForEvaluationAsync();
    Task<string> SaveEvaluationAsync(string applicationId, string hrUserId, int score, string remarks, string passFail, string recommendation);
}