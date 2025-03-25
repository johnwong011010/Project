using Project.Model;
using Project.Service;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<AreaDB>(builder.Configuration.GetSection("AreaDatabase"));
builder.Services.AddSingleton<AreaService>();
builder.Services.Configure<EmployeeDB>(builder.Configuration.GetSection("EmployeeDatabase"));
builder.Services.Configure<Gaming_TablesDB>(builder.Configuration.GetSection("GamingTableDatabase"));
builder.Services.AddSingleton<Gaming_Tables_Service>();
builder.Services.Configure<operationDB>(builder.Configuration.GetSection("OperationDatabase"));
builder.Services.Configure<PitDB>(builder.Configuration.GetSection("PitDatabase"));
builder.Services.AddSingleton<Pit_Service>();
builder.Services.Configure<sub_companiesDB>(builder.Configuration.GetSection("SubCompanyDatabase"));
builder.Services.AddSingleton<Sub_Company_Service>();
builder.Services.Configure<ZoneDB>(builder.Configuration.GetSection("ZoneDatabase"));
builder.Services.AddSingleton<Zone_Service>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
