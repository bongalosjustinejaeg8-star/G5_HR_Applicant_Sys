using HRApplicantSystem.Data.Models;

namespace HRApplicantSystem.Services.Interfaces;

public interface IMyDocumentsService
{
    Task<IEnumerable<ApplicantDocument>> GetApplicantDocumentsAsync();
    Task<bool> UploadDocumentAsync(string applicantId, string fileName, byte[] fileData, string documentType);
    Task<bool> DeleteDocumentAsync(string documentId);
    Task<byte[]?> DownloadDocumentAsync(string documentId);
}
