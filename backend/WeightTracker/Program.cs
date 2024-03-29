using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics;
using WeightTracker.Authentication;
using WeightTracker.Data;
using WeightTracker.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Configuration.AddEnvironmentVariables(prefix: "WeightTracker_");

builder.Services.AddDbContext<WeightTrackerDbContext>(
    options => options.UseNpgsql(ConfigurationExtensions.GetConnectionString(builder.Configuration, "WebAPIDatabaseConnection"))
);
builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<IWeightRepo, WeightRepo>(); 

builder.Services.AddAuthentication().AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

string AnyOriginCorsPolicyName = "AllowAnyOrigin";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AnyOriginCorsPolicyName, builder =>
    {
        builder.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddHealthChecks()
    .AddCheck<EFDbHealthCheck>("EfDbHealthCheck");


var app = builder.Build();

// Check if able to connect to database on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<WeightTrackerDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<WeightTrackerDbContext>>();
    if (!dbContext.Database.CanConnect())
    {
        logger.LogError("Unable to connect to database. Exiting.");
        Environment.Exit(1);
    }

    // Check if there are any pending database migrations and check the environment variable"MIGRATE_DATABASE".
    // If there are pending migrations and MIGRATE_DATABSE is true perform the migrations, else exit.
    _ = bool.TryParse(builder.Configuration.GetValue<string?>("MIGRATE_DATABASE", null), out bool migrateDatabase);
    if (dbContext.Database.GetPendingMigrations().Any())
    {
        if (migrateDatabase != true)
        {
            logger.LogError("There are pending Database Migrations, please set the environment variable \"MIGRATE_DATABASE=True\"");
            Environment.Exit(1);
        }

        try
        {
            logger.LogInformation("Performing database migration.");
            dbContext.Database.Migrate();
            logger.LogInformation("Database migration performed successfully.");
        } catch (Exception ex)
        {
            logger.LogError("Error performing database migration.");
            logger.LogError(ex.ToString());
            Environment.Exit(1);
        }
    }
    logger.LogDebug("Able to connect to database. Proceeding with startup.");
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(AnyOriginCorsPolicyName);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("health", new HealthCheckOptions 
{ 
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();
