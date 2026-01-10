using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PharmacyFinder.API.Data;
using PharmacyFinder.API.Services.Interfaces;
using PharmacyFinder.API.Services.Implementations;
using System.Security.Claims;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add Swagger with JWT support
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Pharmacy Finder API",
        Version = "v1",
        Description = "API for Pharmacy Registration and Medicine Management System"
    });

    // Add JWT Authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token in format: Bearer {token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Database
builder.Services.AddDbContext<ApplsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // object ClaimTypes = null;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        NameClaimType = ClaimTypes.NameIdentifier,
        RoleClaimType = ClaimTypes.Role
    };
});

// Register JWT Service
builder.Services.AddScoped<IJwtService, JwtService>();

// Add Authorization
builder.Services.AddAuthorization();

WebApplication app = builder.Build();

// Database migration
using (IServiceScope scope = app.Services.CreateScope())
{
    try
    {
        ApplsDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplsDbContext>();
        dbContext.Database.Migrate();

        if (dbContext.Database.CanConnect())
        {
            Console.WriteLine("✅ Database connection successful!");
        }
        else
        {
            Console.WriteLine("❌ Unable to connect to the database.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️ Database migration/connection failed: {ex.Message}");
    }
}

if (app.Environment.IsDevelopment())
{
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI();
}

// Order is important!
app.UseHttpsRedirection();
app.UseAuthentication(); // ← Must come before Authorization
app.UseAuthorization();
app.MapControllers();

// Health check
app.MapGet("/", () => "Pharmacy Finder API is running 🚀");

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