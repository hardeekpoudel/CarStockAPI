using FastEndpoints;

public class UpdateStockEndpoint : Endpoint<UpdateStockRequest, UpdateStockResponse>
{
    private readonly CarRepository _carRepository;
    public UpdateStockEndpoint(CarRepository carRepository)
    {
        _carRepository = carRepository;
    }
    public override void Configure()
    {
        Put("/cars/{CarId}/stock");
    }
    public override async Task HandleAsync(UpdateStockRequest req, CancellationToken ct)
    {
        var dealerIdClaim = User.FindFirst("DealerId")?.Value;
        if(!Guid.TryParse(dealerIdClaim, out var dealerId))
        {
            AddError("Invalid or missing DealerId claim");
            await Send.UnauthorizedAsync(cancellation: ct);
            return;
        }

        if(req.Stock < 0)
        {
            AddError("Stock cannot be negative");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        var success = await _carRepository.UpdateStock(dealerId, req.CarId, req.Stock);

        if (!success)
        {
            AddError("Car not found or update failed");
            await Send.NotFoundAsync(cancellation: ct);
            return;
        }
        await Send.OkAsync(new UpdateStockResponse
        {
            Message = "Stock updated successfully"
        }, ct);
    }
}