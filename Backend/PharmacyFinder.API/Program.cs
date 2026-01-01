WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI();
}

app.MapGet("/", () => "Pharmacy Finder API is running 🚀");

// Health check endpoint
app.MapGet("/api/health", () =>
{
    return Results.Ok(new
    {
        Status = "API is running",
        Timestamp = DateTime.UtcNow,
        Version = "1.0.0"
    });
});


app.Run();
