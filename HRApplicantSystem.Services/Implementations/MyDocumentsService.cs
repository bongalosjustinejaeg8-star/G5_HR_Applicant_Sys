using HRApplicantSystem.Data.Models;
using HRApplicantSystem.Data.Repositories;
using HRApplicantSystem.Services.Interfaces;

namespace HRApplicantSystem.Services.Implementations;

public class MyDocumentsService : IMyDocumentsService
{
    private readonly IApplicantDocumentRepository _documentRepository;
    private readonly string _documentStoragePath;

    public MyDocumentsService(IApplicantDocumentRepository documentRepository)
    {
        _documentRepository = documentRepository;
        // Set storage path - in production, this would come from config
        _documentStoragePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Documents");
        
        // Create documents folder if it doesn't exist
        if (!Directory.Exists(_documentStoragePath))
            Directory.CreateDirectory(_documentStoragePath);
    }

    public async Task<IEnumerable<ApplicantDocument>> GetApplicantDocumentsAsync(string applicantId)
    {
        return await _documentRepository.GetByApplicantIdAsync(applicantId);
    }

    public async Task<bool> UploadDocumentAsync(string applicantId, string fileName, byte[] fileData, string documentType)
    {
        // Validate inputs
        if (string.IsNullOrWhiteSpace(fileName) || fileData == null || fileData.Length == 0)
            return false;

        // Validate file size (max 5MB)
        const int maxFileSize = 5 * 1024 * 1024;
        if (fileData.Length > maxFileSize)
            return false;

        // Allowed file types
        var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png" };
        var fileExtension = Path.GetExtension(fileName).ToLower();
        
        if (!allowedExtensions.Contains(fileExtension))
            return false;

        try
        {
            // Generate unique filename
            var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
            var filePath = Path.Combine(_documentStoragePath, uniqueFileName);

            // Save file to disk
            await File.WriteAllBytesAsync(filePath, fileData);

            // Create document record in database
            var document = new ApplicantDocument
            {
                DocumentId = Guid.NewGuid().ToString(),
                ApplicantId = applicantId,
                FilePath = filePath,
                Status = Shared.Enums.DocumentStatus.Submitted,
                SubmittedAt = DateTime.Now,
                RequirementTypeId = documentType
            };

            return await _documentRepository.CreateAsync(document);
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteDocumentAsync(string documentId)
    {
        var document = await _documentRepository.GetByIdAsync(documentId);
        if (document == null) return false;

        try
        {
            // Delete file from disk
            if (File.Exists(document.FilePath))
                File.Delete(document.FilePath);

            // Delete record from database
            return await _documentRepository.DeleteAsync(documentId);
        }
        catch
        {
            return false;
        }
    }

    public async Task<byte[]?> DownloadDocumentAsync(string documentId)
    {
        var document = await _documentRepository.GetByIdAsync(documentId);
        if (document == null) return null;

        try
        {
            if (File.Exists(document.FilePath))
                return await File.ReadAllBytesAsync(document.FilePath);

            return null;
        }
        catch
        {
            return null;
        }
    }
}
