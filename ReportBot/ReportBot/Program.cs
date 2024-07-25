using McgTgBotNet.Models;
using Microsoft.EntityFrameworkCore;
using ReportBot.DataBase.Repositories.Interfaces;
using ReportBot.DataBase.Repositories;
using ReportBot.Services.Services.Interfaces;
using McgTgBotNet.Services;
using Hangfire;
using McgTgBotNet.Hangfire.Extensions;
using ReportBot.Services.Services;
using McgTgBotNet.Profiles;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAutoMapper(typeof(ProjectProfile));

// DbContext
builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Hangfire
builder.Services.AddHangfire(builder.Configuration);

// Repository
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Service
builder.Services.AddScoped<IWorksnapsService, WorksnapsService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IReportService, ReportService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.SetupHangfire();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseHangfireDashboard("/hangfire");

app.Run();
