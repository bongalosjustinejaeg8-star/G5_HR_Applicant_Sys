using HRApplicantSystem.Data.Models;

namespace HRApplicantSystem.Services.Interfaces;

public interface IHiringDecisionService
{
    Task<IEnumerable<Application>> GetApplicationsForHiringDecisionAsync();
    Task<string> MakeHiringDecisionAsync(string applicationId, string hrUserId, string decision, string remarks);
    Task<string> SendOfferLetterAsync(string applicationId, string offerDetails);
    Task<string> SendRejectionLetterAsync(string applicationId, string rejectionReason);
}
