using MySql.Data.MySqlClient;
using HRApplicantSystem.Data.Models;
using HRApplicantSystem.Shared.Enums;

namespace HRApplicantSystem.Data.Repositories;

public class ApplicationRepository : IApplicationRepository
{
    private readonly DbContext _context;

    public ApplicationRepository(DbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Application>> GetAllAsync()
    {
        var applications = new List<Application>();
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = "SELECT application_id, applicant_id, vacancy_id, status, submitted_at, is_locked FROM Applications";
        using var command = new MySqlCommand(query, connection);
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            applications.Add(new Application
            {
                ApplicationId = reader.GetValue(0).ToString()!,
                ApplicantId = reader.GetString(1),
                VacancyId = reader.GetString(2),
                Status = Enum.Parse<ApplicationStatus>(reader.GetString(3)),
                SubmittedAt = reader.GetDateTime(4),
                IsLocked = reader.GetBoolean(5)
            });
        }
        return applications;
    }

    public async Task<Application?> GetByIdAsync(string id)
    {
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = "SELECT application_id, applicant_id, vacancy_id, status, submitted_at, is_locked FROM Applications WHERE application_id = @id";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Application
            {
                ApplicationId = reader.GetValue(0).ToString()!,
                ApplicantId = reader.GetString(1),
                VacancyId = reader.GetString(2),
                Status = Enum.Parse<ApplicationStatus>(reader.GetString(3)),
                SubmittedAt = reader.GetDateTime(4),
                IsLocked = reader.GetBoolean(5)
            };
        }
        return null;
    }

    public async Task<IEnumerable<Application>> GetByApplicantIdAsync(string applicantId)
    {
        var applications = new List<Application>();
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = "SELECT application_id, applicant_id, vacancy_id, status, submitted_at, is_locked FROM Applications WHERE applicant_id = @applicantId";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@applicantId", applicantId);
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            applications.Add(new Application
            {
                ApplicationId = reader.GetValue(0).ToString()!,
                ApplicantId = reader.GetString(1),
                VacancyId = reader.GetString(2),
                Status = Enum.Parse<ApplicationStatus>(reader.GetString(3)),
                SubmittedAt = reader.GetDateTime(4),
                IsLocked = reader.GetBoolean(5)
            });
        }
        return applications;
    }

    // checks if applicant already applied to same vacancy (duplicate prevention)
    public async Task<bool> ExistsAsync(string applicantId, string vacancyId)
    {
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = "SELECT COUNT(*) FROM Applications WHERE applicant_id = @applicantId AND vacancy_id = @vacancyId";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@applicantId", applicantId);
        command.Parameters.AddWithValue("@vacancyId", vacancyId);
        var count = Convert.ToInt32(await command.ExecuteScalarAsync());
        return count > 0;
    }

    public async Task<bool> CreateAsync(Application application)
    {
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = @"INSERT INTO Applications (application_id, applicant_id, vacancy_id, status, submitted_at, is_locked)
                         VALUES (@applicationId, @applicantId, @vacancyId, @status, @submittedAt, @isLocked)";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@applicationId", Guid.NewGuid().ToString());
        command.Parameters.AddWithValue("@applicantId", application.ApplicantId);
        command.Parameters.AddWithValue("@vacancyId", application.VacancyId);
        command.Parameters.AddWithValue("@status", application.Status.ToString());
        command.Parameters.AddWithValue("@submittedAt", DateTime.Now);
        command.Parameters.AddWithValue("@isLocked", application.IsLocked);
        int rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<bool> UpdateStatusAsync(string id, ApplicationStatus status)
    {
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = "UPDATE Applications SET status = @status WHERE application_id = @id";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@status", status.ToString());
        command.Parameters.AddWithValue("@id", id);
        int rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<bool> LockAsync(string id)
    {
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = "UPDATE Applications SET is_locked = TRUE WHERE application_id = @id";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);
        int rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = "DELETE FROM Applications WHERE application_id = @id";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);
        int rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }
}
