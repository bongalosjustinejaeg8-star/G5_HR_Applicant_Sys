using MySql.Data.MySqlClient;
using HRApplicantSystem.Data.Models;
using HRApplicantSystem.Shared.Enums;

namespace HRApplicantSystem.Data.Repositories;

public class ApplicationStatusHistoryRepository : IApplicationStatusHistoryRepository
{
    private readonly DbContext _context;

    public ApplicationStatusHistoryRepository(DbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ApplicationStatusHistory>> GetByApplicationIdAsync(string applicationId)
    {
        var history = new List<ApplicationStatusHistory>();
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = @"SELECT history_id, application_id, changed_by, old_status, new_status, changed_at, remarks 
                         FROM ApplicationStatusHistory 
                         WHERE application_id = @applicationId 
                         ORDER BY changed_at ASC";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@applicationId", applicationId);
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            history.Add(new ApplicationStatusHistory
            {
                HistoryId = reader.GetValue(0).ToString()!,
                ApplicationId = reader.GetString(1),
                ChangedBy = reader.IsDBNull(2) ? null : reader.GetString(2),
                OldStatus = reader.IsDBNull(3) ? null : Enum.Parse<ApplicationStatus>(reader.GetString(3)),
                NewStatus = Enum.Parse<ApplicationStatus>(reader.GetString(4)),
                ChangedAt = reader.GetDateTime(5),
                Remarks = reader.IsDBNull(6) ? null : reader.GetString(6)
            });
        }
        return history;
    }

    public async Task<bool> CreateAsync(ApplicationStatusHistory history)
    {
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = @"INSERT INTO ApplicationStatusHistory 
                         (history_id, application_id, changed_by, old_status, new_status, changed_at, remarks)
                         VALUES (@historyId, @applicationId, @changedBy, @oldStatus, @newStatus, @changedAt, @remarks)";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@historyId", Guid.NewGuid().ToString());
        command.Parameters.AddWithValue("@applicationId", history.ApplicationId);
        command.Parameters.AddWithValue("@changedBy", history.ChangedBy ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@oldStatus", history.OldStatus.HasValue ? history.OldStatus.Value.ToString() : (object)DBNull.Value);
        command.Parameters.AddWithValue("@newStatus", history.NewStatus.ToString());
        command.Parameters.AddWithValue("@changedAt", DateTime.Now);
        command.Parameters.AddWithValue("@remarks", history.Remarks ?? (object)DBNull.Value);
        int rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }
}
