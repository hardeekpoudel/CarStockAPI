using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument(settings =>
{
    settings.EnableJWTBearerAuth = false;

    settings.DocumentSettings = document =>
    {
        document.AddAuth("Bearer", new OpenApiSecurityScheme
        {
            Type = OpenApiSecuritySchemeType.ApiKey,
            Name = "Authorization",
            In = OpenApiSecurityApiKeyLocation.Header,
            Description = "Enter: Bearer {your JWT token}"
        });
    };
});

builder.Services.AddSingleton<DbConnection>();
builder.Services.AddSingleton<DbSchemaInitializer>();

builder.Services.AddScoped<DealerRepository>();
builder.Services.AddScoped<CarRepository>();

builder.Services.AddScoped<PasswordService>();

//JWT secret key
var jwtKey = "CAR_STOCK_API_SECRET_KEY_2026_DEMO_ONLY";

builder.Services.AddScoped<JwtService>(provider =>
{
    return new JwtService(jwtKey);
});

//Jwt authentication configuration
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
                )
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

var dbSchemaInitializer = app.Services.GetRequiredService<DbSchemaInitializer>();
dbSchemaInitializer.Initialize();

app.UseAuthentication();
app.UseAuthorization();

app.UseFastEndpoints();
app.UseSwaggerGen();

app.Run();