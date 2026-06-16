using MySql.Data.MySqlClient;
using HRApplicantSystem.Data.Models;
using HRApplicantSystem.Shared.Enums;

namespace HRApplicantSystem.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DbContext _context;

    public UserRepository(DbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        var users = new List<User>();
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = "SELECT user_id, role_id, username, email, password_hash, is_active FROM Users";
        using var command = new MySqlCommand(query, connection);
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            users.Add(new User
            {
                UserId = Convert.ToString(reader.GetValue(0))!,
                RoleId = Convert.ToString(reader.GetValue(1))!,
                Username = reader.GetString(2),
                Email = reader.GetString(3),
                PasswordHash = reader.GetString(4),
                IsActive = reader.GetBoolean(5)
            });
        }
        return users;
    }

    public async Task<User?> GetByIdAsync(string id)
    {
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = "SELECT user_id, role_id, username, email, password_hash, is_active FROM Users WHERE user_id = @id";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new User
            {
                UserId = Convert.ToString(reader.GetValue(0))!,
                RoleId = Convert.ToString(reader.GetValue(1))!,
                Username = reader.GetString(2),
                Email = reader.GetString(3),
                PasswordHash = reader.GetString(4),
                IsActive = reader.GetBoolean(5)
            };
        }
        return null;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = "SELECT user_id, role_id, username, email, password_hash, is_active FROM Users WHERE email = @email";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@email", email);
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new User
            {
                UserId = Convert.ToString(reader.GetValue(0))!,
                RoleId = Convert.ToString(reader.GetValue(1))!,  
                Username = reader.GetString(2),
                Email = reader.GetString(3),
                PasswordHash = reader.GetString(4),
                IsActive = reader.GetBoolean(5)
            };
        }
        return null;
    }

    public async Task<bool> CreateAsync(User user)
    {
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = @"INSERT INTO Users (user_id, role_id, username, email, password_hash, is_active)
                         VALUES (@userId, @roleId, @username, @email, @passwordHash, @isActive)";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@userId", Guid.NewGuid().ToString());
        command.Parameters.AddWithValue("@roleId", user.RoleId);
        command.Parameters.AddWithValue("@username", user.Username);
        command.Parameters.AddWithValue("@email", user.Email);
        command.Parameters.AddWithValue("@passwordHash", user.PasswordHash);
        command.Parameters.AddWithValue("@isActive", user.IsActive);
        int rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<bool> UpdateAsync(User user)
    {
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = @"UPDATE Users SET role_id = @roleId, username = @username, 
                         email = @email, password_hash = @passwordHash, is_active = @isActive 
                         WHERE user_id = @userId";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@userId", user.UserId);
        command.Parameters.AddWithValue("@roleId", user.RoleId);
        command.Parameters.AddWithValue("@username", user.Username);
        command.Parameters.AddWithValue("@email", user.Email);
        command.Parameters.AddWithValue("@passwordHash", user.PasswordHash);
        command.Parameters.AddWithValue("@isActive", user.IsActive);
        int rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = "DELETE FROM Users WHERE user_id = @id";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);
        int rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }
}
