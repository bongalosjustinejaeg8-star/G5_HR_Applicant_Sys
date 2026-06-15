using MySql.Data.MySqlClient;
using HRApplicantSystem.Data.Models;

namespace HRApplicantSystem.Data.Repositories;

public class RequirementTypeRepository : IRequirementTypeRepository
{
    private readonly DbContext _context;

    public RequirementTypeRepository(DbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RequirementType>> GetAllAsync()
    {
        var types = new List<RequirementType>();
        using var connection = _context.CreateConnection();
        await connection.OpenAsync();

        string query = "SELECT requirement_type_id, name, description FROM requirementtypes ORDER BY name";
        using var command = new MySqlCommand(query, connection);
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            types.Add(new RequirementType
            {
                RequirementTypeId = Convert.ToString(reader["requirement_type_id"]) ?? "",
                Name = Convert.ToString(reader["name"]) ?? "",
                Description = Convert.ToString(reader["description"]) ?? ""
            });
        }
        return types;
    }
}
