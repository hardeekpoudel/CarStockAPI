using Microsoft.Data.Sqlite;

public class DbConnection
{
    private readonly string _connectionString = "Data source=carstock.db";
    public SqliteConnection GetConnection()
    {
        return new SqliteConnection(_connectionString);
    }
}