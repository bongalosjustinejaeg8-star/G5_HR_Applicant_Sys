using HRApplicantSystem.Data.Models;
using HRApplicantSystem.Shared.Enums;

namespace HRApplicantSystem.Services.Interfaces;


public interface IApplicationService
{
    // Applicant side
    Task<bool> SubmitApplicationAsync(string applicantId, string vacancyId);
    Task<bool> CanEditAsync(string applicationId);
    Task<Application?> GetByIdAsync(string applicationId);
    Task<IEnumerable<Application>> GetByApplicantIdAsync(string applicantId);

    // HR side
    Task<bool> StartReviewAsync(string applicationId, string changedBy);
    Task<bool> ChangeStatusAsync(string applicationId, ApplicationStatus newStatus, string changedBy, string? remarks);
    Task<IEnumerable<Application>> GetAllAsync();
}
