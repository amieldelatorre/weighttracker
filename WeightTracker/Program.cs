using Microsoft.EntityFrameworkCore;
using WeightTracker.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<WeightTrackerDBContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("SQLiteConnection")));
builder.Services.AddControllers();
builder.Services.AddScoped<IWeightTrackerAPIRepo, SQLiteDBWebAPIRepo>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
