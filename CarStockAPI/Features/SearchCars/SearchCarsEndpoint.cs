using FastEndpoints;

public class SearchCarsEndpoint : Endpoint<SearchCarsRequest, SearchCarsResponse>
{
    private readonly CarRepository _carRepository;
    public SearchCarsEndpoint(CarRepository carRepository)
    {
        _carRepository = carRepository;
    }
    public override void Configure()
    {
        Get("/cars/search");
    }
    public override async Task HandleAsync(SearchCarsRequest req, CancellationToken ct)
    {
        var dealerIdClaim = User.FindFirst("DealerId")?.Value;
        if(!Guid.TryParse(dealerIdClaim, out var dealerId))
        {
            AddError("Invalid or missing DealerId claim");
            await Send.UnauthorizedAsync(cancellation: ct);
            return;
        }

        if(string.IsNullOrWhiteSpace(req.Make) && string.IsNullOrWhiteSpace(req.Model) && req.Year == null)
        {
            AddError("At least one search parameter (Make, Model, Year) must be provided");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        var cars = await _carRepository.SearchCars(dealerId, req.Make, req.Model, req.Year);
        await Send.OkAsync(new SearchCarsResponse
        {
            Cars = cars
        }, ct);
    }
}