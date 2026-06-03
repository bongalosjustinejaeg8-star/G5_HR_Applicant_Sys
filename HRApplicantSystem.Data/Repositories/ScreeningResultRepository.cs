using MySql.Data.MySqlClient;
using HRApplicantSystem.Data.Models;

namespace HRApplicantSystem.Data.Repositories;

public class ScreeningResultRepository : IScreeningResultRepository
{
    private readonly DbContext _context;

    public ScreeningResultRepository(DbContext context)
    {
        _context = context;
    }

    public async Task<ScreeningResult?> GetByApplicationIdAsync(string applicationId)
    {
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = "SELECT screening_id, application_id, screened_by, result, remarks, screened_at FROM ScreeningResults WHERE application_id = @applicationId";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@applicationId", applicationId);
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new ScreeningResult
            {
                ScreeningId = reader.GetString(0),
                ApplicationId = reader.GetString(1),
                ScreenedBy = reader.IsDBNull(2) ? null : reader.GetString(2),
                Result = reader.GetString(3),
                Remarks = reader.IsDBNull(4) ? null : reader.GetString(4),
                ScreenedAt = reader.GetDateTime(5)
            };
        }
        return null;
    }

    public async Task<bool> CreateAsync(ScreeningResult result)
    {
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = @"INSERT INTO ScreeningResults (screening_id, application_id, screened_by, result, remarks, screened_at)
                         VALUES (@screeningId, @applicationId, @screenedBy, @result, @remarks, @screenedAt)";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@screeningId", Guid.NewGuid().ToString());
        command.Parameters.AddWithValue("@applicationId", result.ApplicationId);
        command.Parameters.AddWithValue("@screenedBy", result.ScreenedBy ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@result", result.Result);
        command.Parameters.AddWithValue("@remarks", result.Remarks ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@screenedAt", DateTime.Now);
        int rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<bool> UpdateAsync(ScreeningResult result)
    {
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = @"UPDATE ScreeningResults SET screened_by = @screenedBy, result = @result, remarks = @remarks
                         WHERE screening_id = @screeningId";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@screeningId", result.ScreeningId);
        command.Parameters.AddWithValue("@screenedBy", result.ScreenedBy ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@result", result.Result);
        command.Parameters.AddWithValue("@remarks", result.Remarks ?? (object)DBNull.Value);
        int rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }
}
