using MySql.Data.MySqlClient;
using HRApplicantSystem.Data.Models;
using HRApplicantSystem.Shared.Enums;

namespace HRApplicantSystem.Data.Repositories;

public class JobVacancyRepository : IJobVacancyRepository
{
    private readonly DbContext _context;

    public JobVacancyRepository(DbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<JobVacancy>> GetAllAsync()
    {
        var vacancies = new List<JobVacancy>();
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = "SELECT vacancy_id, department_id, position_title, qualifications, employment_type, status, created_at FROM JobVacancies";
        using var command = new MySqlCommand(query, connection);
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            vacancies.Add(new JobVacancy
            {
                VacancyId = reader.GetString(0),
                DepartmentId = reader.GetString(1),
                PositionTitle = reader.GetString(2),
                Qualifications = reader.IsDBNull(3) ? null : reader.GetString(3),
                EmploymentType = reader.GetString(4),
                Status = Enum.Parse<VacancyStatus>(reader.GetString(5)),
                CreatedAt = reader.GetDateTime(6)
            });
        }
        return vacancies;
    }

    public async Task<IEnumerable<JobVacancy>> GetOpenAsync()
    {
        var vacancies = new List<JobVacancy>();
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = "SELECT vacancy_id, department_id, position_title, qualifications, employment_type, status, created_at FROM JobVacancies WHERE status = 'Open'";
        using var command = new MySqlCommand(query, connection);
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            vacancies.Add(new JobVacancy
            {
                VacancyId = reader.GetString(0),
                DepartmentId = reader.GetString(1),
                PositionTitle = reader.GetString(2),
                Qualifications = reader.IsDBNull(3) ? null : reader.GetString(3),
                EmploymentType = reader.GetString(4),
                Status = Enum.Parse<VacancyStatus>(reader.GetString(5)),
                CreatedAt = reader.GetDateTime(6)
            });
        }
        return vacancies;
    }

    public async Task<JobVacancy?> GetByIdAsync(string id)
    {
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = "SELECT vacancy_id, department_id, position_title, qualifications, employment_type, status, created_at FROM JobVacancies WHERE vacancy_id = @id";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new JobVacancy
            {
                VacancyId = reader.GetString(0),
                DepartmentId = reader.GetString(1),
                PositionTitle = reader.GetString(2),
                Qualifications = reader.IsDBNull(3) ? null : reader.GetString(3),
                EmploymentType = reader.GetString(4),
                Status = Enum.Parse<VacancyStatus>(reader.GetString(5)),
                CreatedAt = reader.GetDateTime(6)
            };
        }
        return null;
    }

    public async Task<bool> CreateAsync(JobVacancy vacancy)
    {
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = @"INSERT INTO JobVacancies (vacancy_id, department_id, position_title, qualifications, employment_type, status, created_at)
                         VALUES (@vacancyId, @departmentId, @positionTitle, @qualifications, @employmentType, @status, @createdAt)";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@vacancyId", Guid.NewGuid().ToString());
        command.Parameters.AddWithValue("@departmentId", vacancy.DepartmentId);
        command.Parameters.AddWithValue("@positionTitle", vacancy.PositionTitle);
        command.Parameters.AddWithValue("@qualifications", vacancy.Qualifications ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@employmentType", vacancy.EmploymentType);
        command.Parameters.AddWithValue("@status", vacancy.Status.ToString());
        command.Parameters.AddWithValue("@createdAt", DateTime.Now);
        int rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<bool> UpdateAsync(JobVacancy vacancy)
    {
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = @"UPDATE JobVacancies SET department_id = @departmentId, position_title = @positionTitle,
                         qualifications = @qualifications, employment_type = @employmentType, status = @status
                         WHERE vacancy_id = @vacancyId";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@vacancyId", vacancy.VacancyId);
        command.Parameters.AddWithValue("@departmentId", vacancy.DepartmentId);
        command.Parameters.AddWithValue("@positionTitle", vacancy.PositionTitle);
        command.Parameters.AddWithValue("@qualifications", vacancy.Qualifications ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@employmentType", vacancy.EmploymentType);
        command.Parameters.AddWithValue("@status", vacancy.Status.ToString());
        int rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<bool> UpdateStatusAsync(string id, VacancyStatus status)
    {
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = "UPDATE JobVacancies SET status = @status WHERE vacancy_id = @id";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@status", status.ToString());
        command.Parameters.AddWithValue("@id", id);
        int rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = "DELETE FROM JobVacancies WHERE vacancy_id = @id";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);
        int rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }
}
