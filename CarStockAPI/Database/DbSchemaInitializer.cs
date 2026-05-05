using Dapper;
using Microsoft.Data.Sqlite;

public class DbSchemaInitializer
{
    private readonly DbConnection _dbConnection;
    public DbSchemaInitializer(DbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }
    public void Initialize()
    {
        SqliteConnection connection = _dbConnection.GetConnection();
        connection.Open();
        var sql = @"
CREATE TABLE IF NOT EXISTS Dealers(
DealerId TEXT PRIMARY KEY,
DealerName TEXT NOT NULL,
Email TEXT NOT NULL,
PasswordHash TEXT NOT NULL,
InsertDateUtc TEXT NOT NULL,
UpdateDateUtc TEXT NULL
);

CREATE TABLE IF NOT EXISTS Cars(
CarId TEXT PRIMARY KEY,
DealerId TEXT NOT NULL,
Make TEXT NOT NULL,
Model TEXT NOT NULL,
Year INTEGER NOT NULL,
Stock INTEGER NOT NULL,
IsDeleted INTEGER NOT NULL DEFAULT 0,
InsertDateUtc TEXT NOT NULL,
UpdateDateUtc TEXT NULL,
FOREIGN KEY(DealerId) REFERENCES Dealers(DealerId)
);
";
        DynamicParameters parameters = new DynamicParameters();
        connection.Execute(sql, parameters);
    }
}