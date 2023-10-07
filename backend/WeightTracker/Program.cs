using Microsoft.EntityFrameworkCore;
using WeightTracker.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Configuration.AddEnvironmentVariables(prefix: "WeightTracker_");

builder.Services.AddDbContext<WeightTrackerDbContext>(
    options => options.UseNpgsql(ConfigurationExtensions.GetConnectionString(builder.Configuration, "WebAPIDatabaseConnection"))
    //options => options.UseNpgsql(builder.Configuration.GetConnectionString("WebAPIDatabaseConnection"))
);
builder.Services.AddScoped<IUserRepo, PgUserRepo>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
