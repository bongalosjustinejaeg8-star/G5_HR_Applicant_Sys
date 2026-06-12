using HRApplicantSystem.Data.Models;

namespace HRApplicantSystem.Data.Repositories;

public interface IPositionRepository
{
    Task<IEnumerable<Position>> GetAllAsync();
    Task<bool> CreateAsync(Position position);
    Task<bool> UpdateAsync(Position position);
    Task<bool> DeleteAsync(string id);
}

public class PositionRepository : IPositionRepository
{
    private readonly DbContext _context;
    public PositionRepository(DbContext context) { _context = context; }
    public Task<IEnumerable<Position>> GetAllAsync() => Task.FromResult<IEnumerable<Position>>(new List<Position>());
    public Task<bool> CreateAsync(Position position) => Task.FromResult(true);
    public Task<bool> UpdateAsync(Position position) => Task.FromResult(true);
    public Task<bool> DeleteAsync(string id) => Task.FromResult(true);
}
