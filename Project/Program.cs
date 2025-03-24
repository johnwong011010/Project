using Project.Model;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<AreaDB>(builder.Configuration.GetSection("AreaDatabase"));
builder.Services.Configure<EmployeeDB>(builder.Configuration.GetSection("EmployeeDatabase"));
builder.Services.Configure<Gaming_TablesDB>(builder.Configuration.GetSection("GamingTableDatabase"));
builder.Services.Configure<operationDB>(builder.Configuration.GetSection("OperationDatabase"));
builder.Services.Configure<PitDB>(builder.Configuration.GetSection("PitDatabase"));
builder.Services.Configure<sub_companiesDB>(builder.Configuration.GetSection("SubCompanyDatabase"));
builder.Services.Configure<ZoneDB>(builder.Configuration.GetSection("ZoneDatabase"));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
