using MySql.Data.MySqlClient;

namespace HRApplicantSystem.Data;

public class DbContext
{
    private readonly string _connectionString;

    public DbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    public MySqlConnection CreateConnection()
    {
        return new MySqlConnection(_connectionString);
    }
    public bool TestConnection()
    {
        try
        {
            using var connection = CreateConnection();
            connection.Open();
            return true;
        }
        catch
        {
            return false;
        }
    }
}

