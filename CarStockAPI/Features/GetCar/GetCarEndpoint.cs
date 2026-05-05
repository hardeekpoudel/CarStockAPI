using FastEndpoints;

public class GetCarEndpoint : Endpoint<GetCarRequest, GetCarResponse>
{
    private readonly CarRepository _carRepository;
    public GetCarEndpoint(CarRepository carRepository)
    {
        _carRepository = carRepository;
    }
    public override void Configure()
    {
        Get("/cars/{CarId}");
    }
    public override async Task HandleAsync(GetCarRequest req, CancellationToken ct)
    {
        var dealerIdClaim = User.FindFirst("DealerId")?.Value;
        if(!Guid.TryParse(dealerIdClaim, out var dealerId))
        {
            AddError("Invalid or missing DealerId claim");
            await Send.UnauthorizedAsync(cancellation: ct);
            return;
        }

        var car = await _carRepository.GetCarById(dealerId, req.CarId);
        if (car == null)
        {
            AddError("Car not found");
            await Send.NotFoundAsync(cancellation: ct);
            return;
        }
        await Send.OkAsync(new GetCarResponse
        {
            Car = car
        }, ct);
    }
}