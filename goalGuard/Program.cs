using goalGuard;
using goalGuard.Contracts;
using goalGuard.Data;
using goalGuard.Endpoints;
using goalGuard.Http;
using goalGuard.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Refit;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure EF Core
builder.Services.AddDbContext<GoalGuardDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IOnboardingService, OnboardingService>();

// Configure BmoniOptions
builder.Services.Configure<BmoniOptions>(builder.Configuration.GetSection("Bmoni"));

builder.Services.AddTransient<BmoniAuthHeaderHandler>();
builder.Services
    .AddRefitClient<IBmoniApi>()
    .ConfigureHttpClient((provider, client) =>
    {
        var options = provider.GetRequiredService<IOptions<BmoniOptions>>().Value;
        client.BaseAddress = new Uri(options.BaseUrl);
    })
    .AddHttpMessageHandler<BmoniAuthHeaderHandler>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactDev", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173") // common react dev server ports
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("AllowReactDev");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

if (!app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}
app.UseExceptionHandler();

app.UseAuthorization();

app.MapControllers();

app.MapWalletEndpoints();
app.MapTransferEndpoints();
app.MapOnboardingEndpoints();
app.MapHealthEndpoints();

app.Run();
