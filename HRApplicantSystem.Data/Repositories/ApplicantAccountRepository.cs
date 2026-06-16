using MySql.Data.MySqlClient;
using HRApplicantSystem.Data.Models;

namespace HRApplicantSystem.Data.Repositories;

public class ApplicantAccountRepository : IApplicantAccountRepository
{
    private readonly DbContext _context;

    public ApplicantAccountRepository(DbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ApplicantAccount>> GetAllAsync()
    {
        var accounts = new List<ApplicantAccount>();
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = "SELECT account_id, email, password_hash, is_active, created_at FROM ApplicantAccounts";
        using var command = new MySqlCommand(query, connection);
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            accounts.Add(new ApplicantAccount
            {
                AccountId = Convert.ToString(reader.GetValue(0))!,
                Email = Convert.ToString(reader.GetValue(1))!,
                PasswordHash = reader.GetString(2),
                IsActive = reader.GetBoolean(3),
                CreatedAt = reader.GetDateTime(4)
            });
        }
        return accounts;
    }

    public async Task<ApplicantAccount?> GetByIdAsync(string id)
    {
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = "SELECT account_id, email, password_hash, is_active, created_at FROM ApplicantAccounts WHERE account_id = @id";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new ApplicantAccount
            {
                AccountId = Convert.ToString(reader.GetValue(0))!,
                Email = Convert.ToString(reader.GetValue(1))!,
                PasswordHash = reader.GetString(2),
                IsActive = reader.GetBoolean(3),
                CreatedAt = reader.GetDateTime(4)
            };
        }
        return null;
    }

    public async Task<ApplicantAccount?> GetByEmailAsync(string email)
    {
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = "SELECT account_id, email, password_hash, is_active, created_at FROM ApplicantAccounts WHERE email = @email";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@email", email);
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new ApplicantAccount
            {
                AccountId = Convert.ToString(reader.GetValue(0))!,
                Email = Convert.ToString(reader.GetValue(1))!,
                PasswordHash = reader.GetString(2),
                IsActive = reader.GetBoolean(3),
                CreatedAt = reader.GetDateTime(4)
            };
        }
        return null;
    }

    public async Task<bool> CreateAsync(ApplicantAccount account)
    {
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = @"INSERT INTO ApplicantAccounts (account_id, email, password_hash, is_active, created_at)
                         VALUES (@accountId, @email, @passwordHash, @isActive, @createdAt)";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@accountId", Guid.NewGuid().ToString());
        command.Parameters.AddWithValue("@email", account.Email);
        command.Parameters.AddWithValue("@passwordHash", account.PasswordHash);
        command.Parameters.AddWithValue("@isActive", account.IsActive);
        command.Parameters.AddWithValue("@createdAt", DateTime.Now);
        int rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<bool> UpdateAsync(ApplicantAccount account)
    {
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = @"UPDATE ApplicantAccounts SET email = @email, password_hash = @passwordHash, 
                         is_active = @isActive WHERE account_id = @accountId";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@accountId", account.AccountId);
        command.Parameters.AddWithValue("@email", account.Email);
        command.Parameters.AddWithValue("@passwordHash", account.PasswordHash);
        command.Parameters.AddWithValue("@isActive", account.IsActive);
        int rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();
        string query = "DELETE FROM ApplicantAccounts WHERE account_id = @id";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);
        int rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }
}
