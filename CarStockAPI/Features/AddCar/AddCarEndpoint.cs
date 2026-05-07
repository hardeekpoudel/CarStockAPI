using FastEndpoints;

public class AddCarEndpoint : Endpoint<AddCarRequest, AddCarResponse>
{
    private readonly CarRepository _carRepository;
    public AddCarEndpoint(CarRepository carRepository)
    {
        _carRepository = carRepository;
    }
    public override void Configure()
    {
        Post("/cars");
    }
    public override async Task HandleAsync(AddCarRequest req, CancellationToken ct)
    {
        //Get DealerId from Jwt token
        var dealerIdClaim = User.FindFirst("DealerId")?.Value;

        if(!Guid.TryParse(dealerIdClaim, out var dealerId))
        {
            AddError("Invalid or missing dealer ID");
            await Send.UnauthorizedAsync(cancellation: ct);
            return;
        }

        if (string.IsNullOrWhiteSpace(req.Make))
        {
            AddError("Make is required");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        if (string.IsNullOrWhiteSpace(req.Model))
        {
            AddError("Model is required");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        if (req.Year < 1900 || req.Year > DateTime.UtcNow.Year)
        {
            AddError("Year must be between 1900 and current year");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        if (req.Stock < 0)
        {
            AddError("Stock cannot be negative");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        if(req.Price <= 0)
        {
            AddError("Price must be greater than zero");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        var carId = await _carRepository.AddCar(req, dealerId);
        await Send.OkAsync(new AddCarResponse
        {
            CarId = carId,
            Message = "Car added successfully"
        }, ct);
    }
}