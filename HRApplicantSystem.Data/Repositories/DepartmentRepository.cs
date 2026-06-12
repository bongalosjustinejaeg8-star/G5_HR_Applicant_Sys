using HRApplicantSystem.Data.Models;
using MySql.Data.MySqlClient;

namespace HRApplicantSystem.Data.Repositories;

public interface IDepartmentRepository
{
    Task<IEnumerable<Department>> GetAllAsync();
    Task<bool> CreateAsync(Department department);
    Task<bool> UpdateAsync(Department department);
    Task<bool> DeleteAsync(string id);
}

public class DepartmentRepository : IDepartmentRepository
{
    private readonly DbContext _context;
    public DepartmentRepository(DbContext context) { _context = context; }

    public async Task<IEnumerable<Department>> GetAllAsync()
    {
        var list = new List<Department>();
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = "SELECT department_id, department_name FROM Departments";
        using var command = new MySqlCommand(query, connection);
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new Department
            {
                DepartmentId = reader.GetValue(0).ToString()!,
                DepartmentName = reader.GetString(1)
            });
        }
        return list;
    }

    public async Task<bool> CreateAsync(Department department)
    {
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = "INSERT INTO Departments (department_id, department_name) VALUES (@id, @name)";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", Guid.NewGuid().ToString());
        command.Parameters.AddWithValue("@name", department.DepartmentName);
        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> UpdateAsync(Department department)
    {
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = "UPDATE Departments SET department_name = @name WHERE department_id = @id";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@name", department.DepartmentName);
        command.Parameters.AddWithValue("@id", department.DepartmentId);
        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = "DELETE FROM Departments WHERE department_id = @id";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);
        return await command.ExecuteNonQueryAsync() > 0;
    }
}
