using MySql.Data.MySqlClient;
using HRApplicantSystem.Data.Models;
using HRApplicantSystem.Shared.Enums;

namespace HRApplicantSystem.Data.Repositories;

public class InterviewScheduleRepository : IInterviewScheduleRepository
{
    private readonly DbContext _context;

    public InterviewScheduleRepository(DbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<InterviewSchedule>> GetAllAsync()
    {
        var schedules = new List<InterviewSchedule>();
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = "SELECT schedule_id, application_id, interviewer_id, interview_date, mode, location, status FROM InterviewSchedules";
        using var command = new MySqlCommand(query, connection);
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            schedules.Add(new InterviewSchedule
            {
                ScheduleId = Convert.ToString(reader.GetValue(0))!,
                ApplicationId = Convert.ToString(reader.GetValue(1))!,
                InterviewerId = reader.IsDBNull(2) ? null : reader.GetString(2),
                InterviewDate = reader.GetDateTime(3),
                Mode = Enum.Parse<InterviewMode>(reader.GetString(4)),
                Location = reader.IsDBNull(5) ? null : reader.GetString(5),
                Status = Enum.Parse<InterviewStatus>(reader.GetString(6))
            });
        }
        return schedules;
    }

    public async Task<InterviewSchedule?> GetByIdAsync(string id)
    {
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = "SELECT schedule_id, application_id, interviewer_id, interview_date, mode, location, status FROM InterviewSchedules WHERE schedule_id = @id";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new InterviewSchedule
            {
                ScheduleId = Convert.ToString(reader.GetValue(0))!,
                ApplicationId = Convert.ToString(reader.GetValue(1))!,
                InterviewerId = reader.IsDBNull(2) ? null : reader.GetString(2),
                InterviewDate = reader.GetDateTime(3),
                Mode = Enum.Parse<InterviewMode>(reader.GetString(4)),
                Location = reader.IsDBNull(5) ? null : reader.GetString(5),
                Status = Enum.Parse<InterviewStatus>(reader.GetString(6))
            };
        }
        return null;
    }

    public async Task<InterviewSchedule?> GetByApplicationIdAsync(string applicationId)
    {
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = "SELECT schedule_id, application_id, interviewer_id, interview_date, mode, location, status FROM InterviewSchedules WHERE application_id = @applicationId";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@applicationId", applicationId);
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new InterviewSchedule
            {
                ScheduleId = Convert.ToString(reader.GetValue(0))!,
                ApplicationId = Convert.ToString(reader.GetValue(1))!,
                InterviewerId = reader.IsDBNull(2) ? null : reader.GetString(2),
                InterviewDate = reader.GetDateTime(3),
                Mode = Enum.Parse<InterviewMode>(reader.GetString(4)),
                Location = reader.IsDBNull(5) ? null : reader.GetString(5),
                Status = Enum.Parse<InterviewStatus>(reader.GetString(6))
            };
        }
        return null;
    }

    public async Task<bool> CreateAsync(InterviewSchedule schedule)
    {
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = @"INSERT INTO InterviewSchedules (schedule_id, application_id, interviewer_id, interview_date, mode, location, status)
                         VALUES (@scheduleId, @applicationId, @interviewerId, @interviewDate, @mode, @location, @status)";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@scheduleId", Guid.NewGuid().ToString());
        command.Parameters.AddWithValue("@applicationId", schedule.ApplicationId);
        command.Parameters.AddWithValue("@interviewerId", schedule.InterviewerId ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@interviewDate", schedule.InterviewDate);
        command.Parameters.AddWithValue("@mode", schedule.Mode.ToString());
        command.Parameters.AddWithValue("@location", schedule.Location ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@status", schedule.Status.ToString());
        int rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<bool> UpdateAsync(InterviewSchedule schedule)
    {
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = @"UPDATE InterviewSchedules SET interviewer_id = @interviewerId, interview_date = @interviewDate,
                         mode = @mode, location = @location, status = @status
                         WHERE schedule_id = @scheduleId";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@scheduleId", schedule.ScheduleId);
        command.Parameters.AddWithValue("@interviewerId", schedule.InterviewerId ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@interviewDate", schedule.InterviewDate);
        command.Parameters.AddWithValue("@mode", schedule.Mode.ToString());
        command.Parameters.AddWithValue("@location", schedule.Location ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@status", schedule.Status.ToString());
        int rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = "DELETE FROM InterviewSchedules WHERE schedule_id = @id";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);
        int rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }
}
