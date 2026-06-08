using HRApplicantSystem.Data.Models;
using HRApplicantSystem.Shared.Enums;

namespace HRApplicantSystem.Services.Interfaces;

public interface IApplicationService
{
    Task<IEnumerable<Application>> GetByApplicantIdAsync(string applicantId);
    Task<IEnumerable<Application>> GetAllSubmittedAsync();
    Task<string> ApplyAsync(string applicantId, string vacancyId);
    Task<string> SubmitApplicationAsync(string applicationId, string applicantId);
    Task<string> StartReviewAsync(string applicationId, string hrUserId);
}