using FastEndpoints;
using System.Runtime.CompilerServices;

public class RegisterEndpoint : Endpoint<RegisterRequest, RegisterResponse>
{
    private readonly DealerRepository _dealerRepository;
    private readonly PasswordService _passwordService;
    public RegisterEndpoint(DealerRepository dealerRepository, PasswordService passwordService)
    {
        _dealerRepository = dealerRepository;
        _passwordService = passwordService;
    }

    public override void Configure()
    {
        Post("/auth/register");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RegisterRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.DealerName))
        {
            AddError("Dealer name is required");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        if(string.IsNullOrWhiteSpace(req.Email))
        {
            AddError("Email is required");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        if(string.IsNullOrWhiteSpace(req.Password) || req.Password.Length < 8)
        {
            AddError("Password must be atleast 8 characters");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        var emailExists = await _dealerRepository.EmailExists(req.Email);

        if (emailExists) { 
            AddError("Email already exists");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        var passwordHash = _passwordService.HashPassword(req.Password);

        var dealerId = await _dealerRepository.RegisterDealer(
            req.DealerName, 
            req.Email, 
            passwordHash
            );

        await Send.OkAsync(new RegisterResponse 
        { 
            DealerId = dealerId,
            Message = "Dealer registration successful"
        }, cancellation: ct);
    }
}