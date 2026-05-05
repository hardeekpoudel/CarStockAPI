using FastEndpoints;

public class LoginEndpoint : Endpoint<LoginRequest, LoginResponse>
{
    private readonly DealerRepository _dealerRepository;
    private readonly PasswordService _passwordService;
    private readonly JwtService _jwtService;

    public LoginEndpoint(DealerRepository dealerRepository, PasswordService passwordService, JwtService jwtService)
    {
        _dealerRepository = dealerRepository;
        _passwordService = passwordService;
        _jwtService = jwtService;
    }

    public override void Configure()
    {
        Post("/auth/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.Email))
        {
            AddError("Email is required");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }
        if (string.IsNullOrWhiteSpace(req.Password))
        {
            AddError("Password is required");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }
       var dealer = await _dealerRepository.GetDealerByEmail(req.Email);

        if (dealer == null || !_passwordService.VerifyPassword(req.Password, dealer.PasswordHash))
        {
            AddError("Invalid email or password");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        var token = _jwtService.GenerateToken(dealer.DealerId, dealer.Email);
                await Send.OkAsync(new LoginResponse
                {
                    Token = token,
                    Message = "Login successful"
                }, ct);
    }
}