using MySql.Data.MySqlClient;
using HRApplicantSystem.Data.Models;

namespace HRApplicantSystem.Data.Repositories;

public class HiringDecisionRepository : IHiringDecisionRepository
{
    private readonly DbContext _context;

    public HiringDecisionRepository(DbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<HiringDecision>> GetAllAsync()
    {
        var decisions = new List<HiringDecision>();
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = "SELECT decision_id, application_id, decided_by, decision, remarks, decided_at FROM HiringDecisions";
        using var command = new MySqlCommand(query, connection);
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            decisions.Add(new HiringDecision
            {
                DecisionId = reader.GetString(0),
                ApplicationId = reader.GetString(1),
                DecidedBy = reader.IsDBNull(2) ? null : reader.GetString(2),
                Decision = reader.GetString(3),
                Remarks = reader.IsDBNull(4) ? null : reader.GetString(4),
                DecidedAt = reader.GetDateTime(5)
            });
        }
        return decisions;
    }

    public async Task<HiringDecision?> GetByApplicationIdAsync(string applicationId)
    {
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = "SELECT decision_id, application_id, decided_by, decision, remarks, decided_at FROM HiringDecisions WHERE application_id = @applicationId";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@applicationId", applicationId);
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new HiringDecision
            {
                DecisionId = reader.GetString(0),
                ApplicationId = reader.GetString(1),
                DecidedBy = reader.IsDBNull(2) ? null : reader.GetString(2),
                Decision = reader.GetString(3),
                Remarks = reader.IsDBNull(4) ? null : reader.GetString(4),
                DecidedAt = reader.GetDateTime(5)
            };
        }
        return null;
    }

    public async Task<bool> CreateAsync(HiringDecision decision)
    {
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = @"INSERT INTO HiringDecisions (decision_id, application_id, decided_by, decision, remarks, decided_at)
                         VALUES (@decisionId, @applicationId, @decidedBy, @decision, @remarks, @decidedAt)";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@decisionId", Guid.NewGuid().ToString());
        command.Parameters.AddWithValue("@applicationId", decision.ApplicationId);
        command.Parameters.AddWithValue("@decidedBy", decision.DecidedBy ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@decision", decision.Decision);
        command.Parameters.AddWithValue("@remarks", decision.Remarks ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@decidedAt", DateTime.Now);
        int rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<bool> UpdateAsync(HiringDecision decision)
    {
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = @"UPDATE HiringDecisions SET decided_by = @decidedBy, decision = @decision, remarks = @remarks
                         WHERE decision_id = @decisionId";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@decisionId", decision.DecisionId);
        command.Parameters.AddWithValue("@decidedBy", decision.DecidedBy ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@decision", decision.Decision);
        command.Parameters.AddWithValue("@remarks", decision.Remarks ?? (object)DBNull.Value);
        int rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }
}
