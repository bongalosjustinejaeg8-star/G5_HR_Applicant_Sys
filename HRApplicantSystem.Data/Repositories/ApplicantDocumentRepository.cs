using MySql.Data.MySqlClient;
using HRApplicantSystem.Data.Models;
using HRApplicantSystem.Shared.Enums;

namespace HRApplicantSystem.Data.Repositories;

public class ApplicantDocumentRepository : IApplicantDocumentRepository
{
    private readonly DbContext _context;

    public ApplicantDocumentRepository(DbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ApplicantDocument>> GetByApplicantIdAsync(string applicantId)
    {
        var docs = new List<ApplicantDocument>();

        using var connection = _context.CreateConnection();
        await connection.OpenAsync();

        string query = @"
            SELECT document_id, applicant_id, requirement_type_id, file_path, status, remarks, submitted_at
            FROM ApplicantDocuments
            WHERE applicant_id = @applicantId";

        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@applicantId", applicantId);

        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            docs.Add(new ApplicantDocument
            {
                DocumentId = Convert.ToString(reader.GetValue(0)) ?? "",
                ApplicantId = Convert.ToString(reader.GetValue(1)) ?? "",

                // 🔥 FIX: NO GetString (prevents Guid/string crash)
                RequirementTypeId = Convert.ToString(reader.GetValue(2)) ?? "",

                FilePath = Convert.ToString(reader.GetValue(3)) ?? "",

                Status = Enum.TryParse<DocumentStatus>(
                    Convert.ToString(reader.GetValue(4)),
                    out var status)
                    ? status
                    : DocumentStatus.Missing,

                Remarks = reader.IsDBNull(5)
                    ? null
                    : Convert.ToString(reader.GetValue(5)),

                SubmittedAt = reader.IsDBNull(6)
                    ? DateTime.MinValue
                    : reader.GetDateTime(6)
            });
        }

        return docs;
    }

    public async Task<ApplicantDocument?> GetByIdAsync(string documentId)
    {
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();

        string query = @"
            SELECT document_id, applicant_id, requirement_type_id, file_path, status, remarks, submitted_at
            FROM ApplicantDocuments
            WHERE document_id = @id";

        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", documentId);

        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new ApplicantDocument
            {
                DocumentId = Convert.ToString(reader.GetValue(0)) ?? "",
                ApplicantId = Convert.ToString(reader.GetValue(1)) ?? "",

                // 🔥 FIX HERE TOO
                RequirementTypeId = Convert.ToString(reader.GetValue(2)) ?? "",

                FilePath = Convert.ToString(reader.GetValue(3)) ?? "",

                Status = Enum.TryParse<DocumentStatus>(
                    Convert.ToString(reader.GetValue(4)),
                    out var status)
                    ? status
                    : DocumentStatus.Missing,

                Remarks = reader.IsDBNull(5)
                    ? null
                    : Convert.ToString(reader.GetValue(5)),

                SubmittedAt = reader.IsDBNull(6)
                    ? DateTime.MinValue
                    : reader.GetDateTime(6)
            };
        }

        return null;
    }

    public async Task<bool> CreateAsync(ApplicantDocument document)
    {
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();

        string query = @"
            INSERT INTO ApplicantDocuments
            (document_id, applicant_id, requirement_type_id, file_path, status, remarks, submitted_at)
            VALUES
            (@documentId, @applicantId, @requirementTypeId, @filePath, @status, @remarks, @submittedAt)";

        using var command = new MySqlCommand(query, connection);

        command.Parameters.AddWithValue("@documentId", Guid.NewGuid().ToString());
        command.Parameters.AddWithValue("@applicantId", document.ApplicantId);

        // IMPORTANT: ensure string only
        command.Parameters.AddWithValue("@requirementTypeId",
            document.RequirementTypeId?.Trim() ?? "");

        command.Parameters.AddWithValue("@filePath", document.FilePath);
        command.Parameters.AddWithValue("@status", document.Status.ToString());
        command.Parameters.AddWithValue("@remarks", document.Remarks ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@submittedAt", DateTime.Now);

        int rows = await command.ExecuteNonQueryAsync();
        return rows > 0;
    }

    public async Task<bool> DeleteAsync(string documentId)
    {
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();

        string query = "DELETE FROM ApplicantDocuments WHERE document_id = @id";

        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", documentId);

        int rows = await command.ExecuteNonQueryAsync();
        return rows > 0;
    }


    public async Task<bool> UpdateAsync(ApplicantDocument document)
    {
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
    
        string query = @"UPDATE ApplicantDocuments 
                     SET applicant_id = @applicantId, 
                         requirement_type_id = @requirementTypeId, 
                         file_path = @filePath, 
                         status = @status, 
                         remarks = @remarks 
                     WHERE document_id = @documentId";
                     
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@documentId", document.DocumentId);
        command.Parameters.AddWithValue("@applicantId", document.ApplicantId);
        command.Parameters.AddWithValue("@requirementTypeId", document.RequirementTypeId);
        command.Parameters.AddWithValue("@filePath", document.FilePath);
        command.Parameters.AddWithValue("@status", document.Status.ToString());
        command.Parameters.AddWithValue("@remarks", document.Remarks ?? (object)DBNull.Value);
    
        int rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }
}
