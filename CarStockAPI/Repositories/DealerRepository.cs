using Dapper;
using Microsoft.Data.Sqlite;
using System.Data;

public class DealerRepository
{
    private readonly DbConnection _connection;
    public DealerRepository(DbConnection connection)
    {
        _connection = connection;
    }
    public async Task<bool> EmailExists(string email)
    {
        using SqliteConnection connection = _connection.GetConnection();
        connection.Open();

        var sql = @"
SELECT COUNT(1) 
FROM Dealers 
WHERE Email = @Email;
";

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@Email", email, DbType.String, ParameterDirection.Input);
        var count = await connection.ExecuteScalarAsync<int>(sql, parameters);
        return count > 0;
    }

    public async Task<Guid> RegisterDealer(String dealerName, string email, string PasswordHash)
    {
        using SqliteConnection connection = _connection.GetConnection();
        connection.Open();
        var dealerId = Guid.NewGuid();
        var sql = @"
INSERT INTO Dealers (
    DealerId, 
    DealerName, 
    Email, 
    PasswordHash, 
    InsertDateUtc, 
    UpdateDateUtc
) VALUES (
    @DealerId, 
    @DealerName, 
    @Email, 
    @PasswordHash, 
    @InsertDateUtc, 
    @UpdateDateUtc
);
";
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("DealerId", dealerId, DbType.Guid, ParameterDirection.Input);
        parameters.Add("DealerName", dealerName, DbType.String, ParameterDirection.Input);
        parameters.Add("Email", email, DbType.String, ParameterDirection.Input);
        parameters.Add("PasswordHash", PasswordHash, DbType.String, ParameterDirection.Input);
        parameters.Add("InsertDateUtc", DateTime.UtcNow, DbType.DateTime, ParameterDirection.Input);
        parameters.Add("UpdateDateUtc", DateTime.UtcNow, DbType.DateTime, ParameterDirection.Input);
        await connection.ExecuteAsync(sql, parameters);
        return dealerId;
    }

    public async Task<Dealer?> GetDealerByEmail(string email)
    {
        using SqliteConnection connection = _connection.GetConnection();
        var sql = @"
SELECT 
DealerId, 
DealerName, 
Email, 
PasswordHash, 
InsertDateUtc, 
UpdateDateUtc
FROM Dealers 
WHERE 
Email = @Email;
";
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@Email", email, DbType.String, ParameterDirection.Input);

        var result = await connection.QueryFirstOrDefaultAsync<dynamic>(sql, parameters);
        if (result == null)
        {
            return null;
        }

        // Manual mapping
        return new Dealer
        {
            DealerId = Guid.Parse((string)result.DealerId),
            DealerName = result.DealerName,
            Email = result.Email,
            PasswordHash = result.PasswordHash,
            InsertDateUtc = DateTime.Parse((string)result.InsertDateUtc),
            UpdateDateUtc = DateTime.Parse((string)result.UpdateDateUtc)
        };
    }
}