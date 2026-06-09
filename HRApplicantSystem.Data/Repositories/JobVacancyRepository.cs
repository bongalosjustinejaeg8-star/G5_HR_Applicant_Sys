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

    // =========================
    // SAFE MAPPING (CORE FIX)
    // =========================
    private static JobVacancy Map(System.Data.Common.DbDataReader reader)
    {
        var statusRaw = reader.IsDBNull(5)
            ? "Open"
            : reader.GetString(5);

        Enum.TryParse<VacancyStatus>(
            statusRaw.Trim(),
            ignoreCase: true,
            out var status);

        return new JobVacancy
        {
            VacancyId = reader.GetValue(0).ToString()!,
            DepartmentId = reader.GetValue(1).ToString()!,
            PositionTitle = reader.GetString(2),
            Qualifications = reader.IsDBNull(3) ? null : reader.GetString(3),
            EmploymentType = reader.GetString(4),
            Status = status,
            CreatedAt = reader.GetDateTime(6)
        };
    }

    // =========================
    // GET ALL
    // =========================
    public async Task<IEnumerable<JobVacancy>> GetAllAsync()
    {
        var list = new List<JobVacancy>();

        using var conn = _context.CreateConnection();
        await conn.OpenAsync();

        string query = @"
            SELECT vacancy_id, department_id, position_title,
                   qualifications, employment_type, status, created_at
            FROM JobVacancies";

        using var cmd = new MySqlCommand(query, conn);
        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
            list.Add(Map(reader));

        return list;
    }

    // =========================
    // GET OPEN (SAFE VERSION)
    // =========================
    public async Task<IEnumerable<JobVacancy>> GetOpenAsync()
    {
        var list = new List<JobVacancy>();

        using var conn = _context.CreateConnection();
        await conn.OpenAsync();

        // SAFE: normalize case INSIDE SQL
        string query = @"
            SELECT vacancy_id, department_id, position_title,
                   qualifications, employment_type, status, created_at
            FROM JobVacancies
            WHERE LOWER(TRIM(status)) = 'open'";

        using var cmd = new MySqlCommand(query, conn);
        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
            list.Add(Map(reader));

        return list;
    }

    // =========================
    // GET BY ID
    // =========================
    public async Task<JobVacancy?> GetByIdAsync(string id)
    {
        using var conn = _context.CreateConnection();
        await conn.OpenAsync();

        string query = @"
            SELECT vacancy_id, department_id, position_title,
                   qualifications, employment_type, status, created_at
            FROM JobVacancies
            WHERE vacancy_id = @id";

        using var cmd = new MySqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@id", id);

        using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
            return Map(reader);

        return null;
    }

    // =========================
    // CREATE
    // =========================
    public async Task<bool> CreateAsync(JobVacancy vacancy)
    {
        using var conn = _context.CreateConnection();
        await conn.OpenAsync();

        string query = @"
            INSERT INTO JobVacancies
            (vacancy_id, department_id, position_title,
             qualifications, employment_type, status, created_at)
            VALUES
            (@id, @dept, @title, @qual, @type, @status, @created)";

        using var cmd = new MySqlCommand(query, conn);

        cmd.Parameters.AddWithValue("@id", Guid.NewGuid().ToString());
        cmd.Parameters.AddWithValue("@dept", vacancy.DepartmentId);
        cmd.Parameters.AddWithValue("@title", vacancy.PositionTitle);
        cmd.Parameters.AddWithValue("@qual", vacancy.Qualifications ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@type", vacancy.EmploymentType);
        cmd.Parameters.AddWithValue("@status", vacancy.Status.ToString());
        cmd.Parameters.AddWithValue("@created", DateTime.Now);

        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    // =========================
    // UPDATE
    // =========================
    public async Task<bool> UpdateAsync(JobVacancy vacancy)
    {
        using var conn = _context.CreateConnection();
        await conn.OpenAsync();

        string query = @"
            UPDATE JobVacancies
            SET department_id = @dept,
                position_title = @title,
                qualifications = @qual,
                employment_type = @type,
                status = @status
            WHERE vacancy_id = @id";

        using var cmd = new MySqlCommand(query, conn);

        cmd.Parameters.AddWithValue("@id", vacancy.VacancyId);
        cmd.Parameters.AddWithValue("@dept", vacancy.DepartmentId);
        cmd.Parameters.AddWithValue("@title", vacancy.PositionTitle);
        cmd.Parameters.AddWithValue("@qual", vacancy.Qualifications ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@type", vacancy.EmploymentType);
        cmd.Parameters.AddWithValue("@status", vacancy.Status.ToString());

        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    // =========================
    // UPDATE STATUS (SAFE)
    // =========================
    public async Task<bool> UpdateStatusAsync(string id, VacancyStatus status)
    {
        using var conn = _context.CreateConnection();
        await conn.OpenAsync();

        string query = @"
            UPDATE JobVacancies
            SET status = @status
            WHERE vacancy_id = @id";

        using var cmd = new MySqlCommand(query, conn);

        cmd.Parameters.AddWithValue("@status", status.ToString());
        cmd.Parameters.AddWithValue("@id", id);

        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    // =========================
    // DELETE
    // =========================
    public async Task<bool> DeleteAsync(string id)
    {
        using var conn = _context.CreateConnection();
        await conn.OpenAsync();

        string query = "DELETE FROM JobVacancies WHERE vacancy_id = @id";

        using var cmd = new MySqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@id", id);

        return await cmd.ExecuteNonQueryAsync() > 0;
    }
}