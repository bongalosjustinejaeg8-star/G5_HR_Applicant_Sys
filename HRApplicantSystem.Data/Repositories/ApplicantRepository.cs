using MySql.Data.MySqlClient;
using HRApplicantSystem.Data.Models;

namespace HRApplicantSystem.Data.Repositories;

public class ApplicantRepository : IApplicantRepository
{
    private readonly DbContext _context;

    public ApplicantRepository(DbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Applicant>> GetAllAsync()
    {
        var applicants = new List<Applicant>();
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = "SELECT applicant_id, account_id, full_name, address, contact_no, education, work_experience, skills FROM Applicants";
        using var command = new MySqlCommand(query, connection);
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            applicants.Add(new Applicant
            {
                ApplicantId = Convert.ToString(reader.GetValue(0))!,
                AccountId = Convert.ToString(reader.GetValue(1))!,
                FullName = reader.GetString(2),
                Address = reader.IsDBNull(3) ? null : reader.GetString(3),
                ContactNo = reader.IsDBNull(4) ? null : reader.GetString(4),
                Education = reader.IsDBNull(5) ? null : reader.GetString(5),
                WorkExperience = reader.IsDBNull(6) ? null : reader.GetString(6),
                Skills = reader.IsDBNull(7) ? null : reader.GetString(7)
            });
        }
        return applicants;
    }

    public async Task<Applicant?> GetByIdAsync(string id)
    {
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = "SELECT applicant_id, account_id, full_name, address, contact_no, education, work_experience, skills FROM Applicants WHERE applicant_id = @id";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Applicant
            {
                ApplicantId = Convert.ToString(reader.GetValue(0))!,
                AccountId = Convert.ToString(reader.GetValue(1))!,
                FullName = reader.GetString(2),
                Address = reader.IsDBNull(3) ? null : reader.GetString(3),
                ContactNo = reader.IsDBNull(4) ? null : reader.GetString(4),
                Education = reader.IsDBNull(5) ? null : reader.GetString(5),
                WorkExperience = reader.IsDBNull(6) ? null : reader.GetString(6),
                Skills = reader.IsDBNull(7) ? null : reader.GetString(7)
            };
        }
        return null;
    }

    public async Task<Applicant?> GetByAccountIdAsync(string accountId)
    {
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = "SELECT applicant_id, account_id, full_name, address, contact_no, education, work_experience, skills FROM Applicants WHERE account_id = @accountId";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@accountId", accountId);
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Applicant
            {
                ApplicantId = Convert.ToString(reader.GetValue(0))!,
                AccountId = Convert.ToString(reader.GetValue(1))!,
                FullName = reader.GetString(2),
                Address = reader.IsDBNull(3) ? null : reader.GetString(3),
                ContactNo = reader.IsDBNull(4) ? null : reader.GetString(4),
                Education = reader.IsDBNull(5) ? null : reader.GetString(5),
                WorkExperience = reader.IsDBNull(6) ? null : reader.GetString(6),
                Skills = reader.IsDBNull(7) ? null : reader.GetString(7)
            };
        }
        return null;
    }

    public async Task<bool> CreateAsync(Applicant applicant)
    {
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = @"INSERT INTO Applicants (applicant_id, account_id, full_name, address, contact_no, education, work_experience, skills)
                         VALUES (@applicantId, @accountId, @fullName, @address, @contactNo, @education, @workExperience, @skills)";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@applicantId", Guid.NewGuid().ToString());
        command.Parameters.AddWithValue("@accountId", applicant.AccountId);
        command.Parameters.AddWithValue("@fullName", applicant.FullName);
        command.Parameters.AddWithValue("@address", applicant.Address ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@contactNo", applicant.ContactNo ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@education", applicant.Education ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@workExperience", applicant.WorkExperience ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@skills", applicant.Skills ?? (object)DBNull.Value);
        int rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<bool> UpdateAsync(Applicant applicant)
    {
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = @"UPDATE Applicants SET full_name = @fullName, address = @address, contact_no = @contactNo,
                         education = @education, work_experience = @workExperience, skills = @skills
                         WHERE applicant_id = @applicantId";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@applicantId", applicant.ApplicantId);
        command.Parameters.AddWithValue("@fullName", applicant.FullName);
        command.Parameters.AddWithValue("@address", applicant.Address ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@contactNo", applicant.ContactNo ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@education", applicant.Education ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@workExperience", applicant.WorkExperience ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@skills", applicant.Skills ?? (object)DBNull.Value);
        int rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = "DELETE FROM Applicants WHERE applicant_id = @id";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);
        int rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }
}
