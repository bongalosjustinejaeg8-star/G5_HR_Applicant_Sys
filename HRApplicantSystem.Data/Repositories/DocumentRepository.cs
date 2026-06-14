using HRApplicantSystem.Data.Models;
using MySql.Data.MySqlClient;
public class DocumentRepository
{
    private readonly DbContext _db;
    public DocumentRepository(DbContext db) => _db = db;

    public async Task<List<Document>> GetByApplicantIdAsync(string applicantId) {  }
    public async Task AddAsync(Document doc) {  }
    public async Task DeleteAsync(int documentId) {  }
}