using HRApplicantSystem.Data.Models;

namespace HRApplicantSystem.Data.Repositories;

public interface IApplicantDocumentRepository
{
    Task<IEnumerable<ApplicantDocument>> GetByApplicantIdAsync(string applicantId);
    Task<ApplicantDocument?> GetByIdAsync(string documentId);
    Task<bool> CreateAsync(ApplicantDocument document);
    Task<bool> DeleteAsync(string documentId);
}