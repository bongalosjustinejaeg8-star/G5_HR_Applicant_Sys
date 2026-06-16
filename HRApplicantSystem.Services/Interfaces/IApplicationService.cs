using HRApplicantSystem.Data.Models;
using HRApplicantSystem.Shared.Enums;

namespace HRApplicantSystem.Services.Interfaces;

public interface IApplicationService
{
    // Applicant side — no userId needed, changed_by is null for applicant actions
    Task<bool> SubmitApplicationAsync(string applicantId, string vacancyId);
    Task<bool> CanEditAsync(string applicationId);
    Task<Application?> GetByIdAsync(string applicationId);
    Task<IEnumerable<Application>> GetByApplicantIdAsync(string applicantId);

    // HR side — changedBy must be a valid user_id from the Users table
    Task<bool> StartReviewAsync(string applicationId, string changedBy);
    Task<bool> ChangeStatusAsync(string applicationId, ApplicationStatus newStatus, string changedBy, string? remarks);
    Task<IEnumerable<Application>> GetAllAsync();
}