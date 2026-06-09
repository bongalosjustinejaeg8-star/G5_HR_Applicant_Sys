using MySql.Data.MySqlClient;
using HRApplicantSystem.Data.Models;

namespace HRApplicantSystem.Data.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly DbContext _context;

    public RoleRepository(DbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<Role>> GetAllAsync()
    {
        var roles = new List<Role>();

        using var connection = _context.CreateConnection();
        await connection.OpenAsync();

        string query = "SELECT role_id, role_name, permissions FROM Roles";

        using var command = new MySqlCommand(query, connection);
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            roles.Add(new Role
            {
                RoleId = reader.GetValue(0).ToString()!,
                RoleName = reader.GetString(1),
                Permissions = reader.IsDBNull(2)
                              ? null
                              : reader.GetString(2)
            });
        }

        return roles;
    }

    public async Task<Role?> GetByIdAsync(string id)
    {
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = "SELECT role_id, role_name, permissions FROM Roles WHERE role_id = @id";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Role
            {
                RoleId = reader.GetValue(0).ToString()!,
                RoleName = reader.GetString(1),
                Permissions = reader.IsDBNull(2)
                              ? null
                              : reader.GetString(2)
            };
        }
        return null;
    }

    public async Task<bool> CreateAsync(Role role)
    {
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = @"INSERT INTO Roles (role_id, role_name, permissions) 
                     VALUES (@roleId, @roleName, @permissions)";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@roleId", Guid.NewGuid().ToString());
        command.Parameters.AddWithValue("@roleName", role.RoleName);
        command.Parameters.AddWithValue("@permissions", role.Permissions ?? (object)DBNull.Value);
        int rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = "DELETE FROM Roles WHERE role_id = @id";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);
        int rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }
}