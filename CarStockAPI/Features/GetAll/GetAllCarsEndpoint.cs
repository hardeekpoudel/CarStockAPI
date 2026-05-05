using FastEndpoints;

public class GetAllCarsEndpoint : EndpointWithoutRequest<GetAllCarsResponse>
{
    private readonly CarRepository _carRepository;
    public GetAllCarsEndpoint(CarRepository carRepository)
    {
        _carRepository = carRepository;
    }
    public override void Configure()
    {
        Get("/cars");
    }
    public override async Task HandleAsync(CancellationToken ct)
    {
        //Get dealerId from Jwt token
        var dealerIdClaim = User.FindFirst("DealerId")?.Value;

        if(!Guid.TryParse(dealerIdClaim, out var dealerId))
        {
            AddError("Invalid or missing dealer ID");
            await Send.UnauthorizedAsync(cancellation: ct);
            return;
        }

        var cars = await _carRepository.GetAllCars(dealerId);

        await Send.OkAsync(new GetAllCarsResponse
        {
            Cars = cars
        }, ct);
    }
}