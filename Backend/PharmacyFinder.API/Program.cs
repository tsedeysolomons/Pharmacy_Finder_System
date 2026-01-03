using Microsoft.EntityFrameworkCore;
using PharmacyFinder.API.Data;
//using NetTopologySuite.Geometries;


WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AppsDbContext"),
    sqlOptions => sqlOptions.UseNetTopologySuite()
));
WebApplication app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope())
{
    ApplsDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplsDbContext>();
    dbContext.Database.Migrate();
    if (!dbContext.Database.CanConnect())
    {
        // Database connection successful
        throw new NotImplementedException("Unable to connect to the database.");
    }
    else
    {
        Console.WriteLine("Database connection successful!");   // Handle the case where the database connection fails
    }
}

WebApplication tempapp = builder.Build();

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
