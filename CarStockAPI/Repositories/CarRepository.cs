using Dapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using System.Data;

public class CarRepository
{
    private readonly DbConnection _connection;
    public CarRepository(DbConnection connection)
    {
        _connection = connection;
    }
    public async Task<Guid> AddCar(AddCarRequest req, Guid dealerId)
    {
        SqliteConnection connection = _connection.GetConnection();
        connection.Open();

        var carId = Guid.NewGuid();

        var sql = @"
INSERT INTO Cars (
CarId, 
DealerId,
Make, 
Model, 
Year, 
Stock,
Price,
IsDeleted,
InsertDateUtc,
UpdateDateUtc
)
VALUES (
@CarId,
@DealerId,
@Make, 
@Model,
@Year, 
@Stock,
@Price,
@IsDeleted,
@InsertDateUtc,
@UpdateDateUtc
)
";

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@CarId", carId, DbType.Guid, ParameterDirection.Input);
        parameters.Add("@DealerId", dealerId, DbType.Guid, ParameterDirection.Input);
        parameters.Add("@Make", req.Make, DbType.String, ParameterDirection.Input);
        parameters.Add("@Model", req.Model, DbType.String, ParameterDirection.Input);
        parameters.Add("@Year", req.Year, DbType.Int32, ParameterDirection.Input);
        parameters.Add("@Stock", req.Stock, DbType.Int32, ParameterDirection.Input);
        parameters.Add("@Price", req.Price, DbType.Decimal, ParameterDirection.Input);
        parameters.Add("@IsDeleted", false, DbType.Boolean, ParameterDirection.Input);
        parameters.Add("@InsertDateUtc", DateTime.UtcNow, DbType.DateTime, ParameterDirection.Input);
        parameters.Add("@UpdateDateUtc", DateTime.UtcNow, DbType.DateTime, ParameterDirection.Input);

        await connection.ExecuteAsync(sql, parameters);

        return carId;
    }

    public async Task<CarDto?> GetCarById(Guid dealerId, Guid carId)
    {
        using SqliteConnection connection = _connection.GetConnection();

        var sql = @"
SELECT CarId, Make, Model, Year, Stock, Price
FROM Cars
WHERE DealerId = @DealerId 
AND CarId = @CarId
AND IsDeleted = 0
";
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@DealerId", dealerId, DbType.Guid, ParameterDirection.Input);
        parameters.Add("@CarId", carId, DbType.Guid, ParameterDirection.Input);
        
        var result = await connection.QueryFirstOrDefaultAsync<dynamic>(sql, parameters);


        if (result == null)
        {
            return null;
        }

        return new CarDto
        {
            CarId = Guid.Parse(result.CarId.ToString()),
            Make = result.Make,
            Model = result.Model,
            Year = (int)result.Year,
            Stock = (int)result.Stock,
            Price = (Decimal?)result.Price
        };
    }

    public async Task<List<CarDto>> GetAllCars(Guid dealerId)
    {
        SqliteConnection connection = _connection.GetConnection();
        connection.Open();
        var sql = @"
SELECT CarId, Make, Model, Year, Stock, Price
FROM Cars
WHERE DealerId = @DealerId
AND IsDeleted = 0
ORDER BY Make, Model, Year;
";
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@DealerId", dealerId, DbType.Guid, ParameterDirection.Input);

        var results = await connection.QueryAsync<dynamic>(sql, parameters);

        var cars = results.Select(result => new CarDto
        {
            CarId = Guid.Parse((string)result.CarId),
            Make = result.Make,
            Model = result.Model,
            Year = (int)result.Year,
            Stock = (int)result.Stock,
            Price = (Decimal?)result.Price
        }).ToList();

        return cars;
    }

    public async Task<List<CarDto>> SearchCars(Guid dealerId, string? make, string? model, int? year)
    {
        SqliteConnection connection = _connection.GetConnection();
        connection.Open();
        var sql = @"
SELECT CarId, Make, Model, Year, Stock, Price
FROM Cars
WHERE DealerId = @DealerId
AND IsDeleted = 0
AND (@Make IS NULL OR Make LIKE '%' || @Make || '%')
AND (@Model IS NULL OR Model LIKE '%' || @Model || '%')
AND (@Year IS NULL OR Year = @Year)
ORDER BY Make, Model, Year;
";
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@DealerId", dealerId, DbType.Guid, ParameterDirection.Input);
        parameters.Add("@Make", string.IsNullOrWhiteSpace(make) ? null : make, DbType.String, ParameterDirection.Input);
        parameters.Add("@Model", string.IsNullOrWhiteSpace(model) ? null : model, DbType.String, ParameterDirection.Input);
        parameters.Add("@Year", year, DbType.Int32, ParameterDirection.Input);

        var results = await connection.QueryAsync<dynamic>(sql, parameters);

        var cars = results.Select(result => new CarDto
        {
            CarId = Guid.Parse(result.CarId.ToString()),
            Make = result.Make,
            Model = result.Model,
            Year = Convert.ToInt32(result.Year),
            Stock = Convert.ToInt32(result.Stock),
            Price = Convert.ToDecimal(result.Price)
        }).ToList();

        return cars;
    }

    public async Task<bool> UpdateStock(Guid dealerId, Guid carId, int stock)
    {
        SqliteConnection connection = _connection.GetConnection();
        connection.Open();
        var sql = @"
UPDATE Cars
SET Stock = @Stock,
UpdateDateUtc = @UpdateDateUtc
WHERE DealerId = @DealerId
AND CarId = @CarId
AND IsDeleted = 0
";
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@Stock", stock, DbType.Int32, ParameterDirection.Input);
        parameters.Add("@UpdateDateUtc", DateTime.UtcNow, DbType.DateTime, ParameterDirection.Input);
        parameters.Add("@DealerId", dealerId, DbType.Guid, ParameterDirection.Input);
        parameters.Add("@CarId", carId, DbType.Guid, ParameterDirection.Input);

        var rowsAffected = await connection.ExecuteAsync(sql, parameters);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteCar(Guid dealerId, Guid carId)
    {
        SqliteConnection connection = _connection.GetConnection();
        connection.Open();
        var sql = @"
UPDATE Cars
SET IsDeleted = 1,
UpdateDateUtc = @UpdateDateUtc
WHERE DealerId = @DealerId
AND CarId = @CarId
AND IsDeleted = 0
";
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@UpdateDateUtc", DateTime.UtcNow, DbType.DateTime, ParameterDirection.Input);
        parameters.Add("@DealerId", dealerId, DbType.Guid, ParameterDirection.Input);
        parameters.Add("@CarId", carId, DbType.Guid, ParameterDirection.Input);
        
        var rowsAffected = await connection.ExecuteAsync(sql, parameters);
        return rowsAffected > 0;
    }
}