using MySql.Data.MySqlClient;
using HRApplicantSystem.Data.Models;

namespace HRApplicantSystem.Data.Repositories;

public class InterviewEvaluationRepository : IInterviewEvaluationRepository
{
    private readonly DbContext _context;

    public InterviewEvaluationRepository(DbContext context)
    {
        _context = context;
    }

    public async Task<InterviewEvaluation?> GetByScheduleIdAsync(string scheduleId)
    {
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = "SELECT eval_id, schedule_id, evaluated_by, score, remarks, recommendation, pass_fail FROM InterviewEvaluations WHERE schedule_id = @scheduleId";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@scheduleId", scheduleId);
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new InterviewEvaluation
            {
                EvalId = reader.GetValue(0).ToString()!,
                ScheduleId = reader.GetString(1),
                EvaluatedBy = reader.IsDBNull(2) ? null : reader.GetString(2),
                Score = reader.IsDBNull(3) ? null : reader.GetInt32(3),
                Remarks = reader.IsDBNull(4) ? null : reader.GetString(4),
                Recommendation = reader.IsDBNull(5) ? null : reader.GetString(5),
                PassFail = reader.GetBoolean(6).ToString()
            };
        }
        return null;
    }

    public async Task<bool> CreateAsync(InterviewEvaluation evaluation)
    {
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = @"INSERT INTO InterviewEvaluations (eval_id, schedule_id, evaluated_by, score, remarks, recommendation, pass_fail)
                         VALUES (@evalId, @scheduleId, @evaluatedBy, @score, @remarks, @recommendation, @passFail)";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@evalId", Guid.NewGuid().ToString());
        command.Parameters.AddWithValue("@scheduleId", evaluation.ScheduleId);
        command.Parameters.AddWithValue("@evaluatedBy", evaluation.EvaluatedBy ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@score", evaluation.Score ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@remarks", evaluation.Remarks ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@recommendation", evaluation.Recommendation ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@passFail", evaluation.PassFail);
        int rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<bool> UpdateAsync(InterviewEvaluation evaluation)
    {
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = @"UPDATE InterviewEvaluations SET evaluated_by = @evaluatedBy, score = @score,
                         remarks = @remarks, recommendation = @recommendation, pass_fail = @passFail
                         WHERE eval_id = @evalId";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@evalId", evaluation.EvalId);
        command.Parameters.AddWithValue("@evaluatedBy", evaluation.EvaluatedBy ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@score", evaluation.Score ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@remarks", evaluation.Remarks ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@recommendation", evaluation.Recommendation ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@passFail", evaluation.PassFail);
        int rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }
}
