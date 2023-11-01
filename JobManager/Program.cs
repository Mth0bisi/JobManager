using JobManager;
using JobManager.Data;
using JobManager.Data.Repository;
using JobManager.Models;
using JobManager.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<JobManagerDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("JobManagerDbContext"));
});
builder.Services.AddHttpClient();
builder.Services.AddScoped<IJobService, JobManagementService>();
builder.Services.AddScoped<IGenericRepository<Job>, GenericRepository<Job>>();
builder.Services.AddScoped<IGenericRepository<WeatherForecast>, GenericRepository<WeatherForecast>>();
builder.Services.AddHostedService<BackgroundJobService>();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHealthChecks()
    .AddSqlServer(builder.Configuration.GetConnectionString("JobManagerDbContext"));

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

app.UseSerilogRequestLogging();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapHealthChecks("api/health");

app.UseAuthorization();

app.MapControllers();

app.Run();
