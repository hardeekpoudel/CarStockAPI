using FastEndpoints;

public class DeleteCarEndpoint : Endpoint<DeleteCarRequest>
{
    private readonly CarRepository _carRepository;
    public DeleteCarEndpoint(CarRepository carRepository)
    {
        _carRepository = carRepository;
    }
    public override void Configure()
    {
        Delete("/cars/{CarId}");
    }
    public override async Task HandleAsync(DeleteCarRequest req, CancellationToken ct)
    {
        var dealerIdClaim = User.FindFirst("DealerId")?.Value;
        if(!Guid.TryParse(dealerIdClaim, out var dealerId))
        {
            AddError("Invalid or missing DealerId claim");
            await Send.UnauthorizedAsync(cancellation: ct);
            return;
        }

        var success = await _carRepository.DeleteCar(dealerId, req.CarId);
        if (!success)
        {
            AddError("Car not found or could not be deleted");
            await Send.NotFoundAsync(cancellation: ct);
            return;
        }
        await Send.OkAsync(new DeleteCarResponse
        {
            Message = "Car deleted successfully"
        }, ct);
    }
}