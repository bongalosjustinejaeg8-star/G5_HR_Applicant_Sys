using HRApplicantSystem.Data.Models;

namespace HRApplicantSystem.Services.Interfaces;

public interface IProfileService
{
    Task<ApplicantAccount?> GetProfileAsync(string applicantId);
    Task<bool> UpdateProfileAsync(ApplicantAccount profile);
    Task<bool> UploadProfilePhotoAsync(string applicantId, byte[] photoData);
}
