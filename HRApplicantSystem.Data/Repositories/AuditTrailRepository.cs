using MySql.Data.MySqlClient;
using HRApplicantSystem.Data.Models;

namespace HRApplicantSystem.Data.Repositories;

public class AuditTrailRepository : IAuditTrailRepository
{
    private readonly DbContext _context;

    public AuditTrailRepository(DbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AuditTrail>> GetAllAsync()
    {
        var audits = new List<AuditTrail>();
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = "SELECT audit_id, user_id, action, table_affected, record_id, performed_at FROM AuditTrail ORDER BY performed_at DESC";
        using var command = new MySqlCommand(query, connection);
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            audits.Add(new AuditTrail
            {
                AuditId = reader.GetValue(0).ToString()!,
                UserId = reader.IsDBNull(1) ? null : reader.GetValue(1).ToString()!,
                Action = reader.GetString(2),
                TableAffected = reader.GetString(3),
                RecordId = reader.GetString(4),
                PerformedAt = reader.GetDateTime(5)
            });
        }
        return audits;
    }

    public async Task<IEnumerable<AuditTrail>> GetByUserIdAsync(string userId)
    {
        var audits = new List<AuditTrail>();
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = "SELECT audit_id, user_id, action, table_affected, record_id, performed_at FROM AuditTrail WHERE user_id = @userId ORDER BY performed_at DESC";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@userId", userId);
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            audits.Add(new AuditTrail
            {
                AuditId = reader.GetValue(0).ToString()!,
                UserId = reader.IsDBNull(1) ? "" : reader.GetValue(1).ToString() ?? "",
                Action = reader.GetString(2),
                TableAffected = reader.GetString(3),
                RecordId = reader.GetString(4),
                PerformedAt = reader.GetDateTime(5)
            });
        }
        return audits;
    }

    public async Task<bool> CreateAsync(AuditTrail audit)
    {
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = @"INSERT INTO AuditTrail (audit_id, user_id, action, table_affected, record_id, performed_at)
                         VALUES (@auditId, @userId, @action, @tableAffected, @recordId, @performedAt)";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@auditId", Guid.NewGuid().ToString());
        command.Parameters.AddWithValue("@userId", audit.UserId ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@action", audit.Action);
        command.Parameters.AddWithValue("@tableAffected", audit.TableAffected);
        command.Parameters.AddWithValue("@recordId", audit.RecordId);
        command.Parameters.AddWithValue("@performedAt", DateTime.Now);
        int rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }
}
