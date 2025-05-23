using Project.Model;
using Project.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<AreaDB>(builder.Configuration.GetSection("AreaDatabase"));
builder.Services.AddSingleton<AreaService>();
builder.Services.Configure<EmployeeDB>(builder.Configuration.GetSection("EmployeeDatabase"));
builder.Services.AddSingleton<EmployeeService>();
builder.Services.AddSingleton<JwtService>();
builder.Services.AddSingleton<IRefreshTokenRepository, RefreshTokenRepositoryClass>();
builder.Services.Configure<Gaming_TablesDB>(builder.Configuration.GetSection("GamingTableDatabase"));
builder.Services.Configure<ChipTypeDb>(builder.Configuration.GetSection("Chips_type"));
builder.Services.Configure<denominationDB>(builder.Configuration.GetSection("Denominations"));
builder.Services.AddSingleton<Gaming_Tables_Service>();
builder.Services.Configure<operationDB>(builder.Configuration.GetSection("OperationDatabase"));
builder.Services.AddSingleton<OperationLogService>();
builder.Services.Configure<PitDB>(builder.Configuration.GetSection("PitDatabase"));
builder.Services.AddSingleton<Pit_Service>();
builder.Services.Configure<sub_companiesDB>(builder.Configuration.GetSection("SubCompanyDatabase"));
builder.Services.AddSingleton<Sub_Company_Service>();
builder.Services.Configure<ZoneDB>(builder.Configuration.GetSection("ZoneDatabase"));
builder.Services.AddSingleton<Zone_Service>();
builder.Services.Configure<PermissionDB>(builder.Configuration.GetSection("PermissionDatabase"));//動態監視permission的DB設定
var jwtsetting = builder.Configuration.GetSection("Jwt");//get Jwt setting

//set up the authentication is using json web token
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,//active the jwt lifetime
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],//config the issuser
        //ValidAudience = builder.Configuration["Jwt:Audience"],//config the audience
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))//turn both public and private key to utf8 encoding format
    };
});
//允許跨域訪問資源設定
builder.Services.AddCors(option =>
{
    option.AddPolicy("ReactAppPolicy", builder => builder.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin());
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});

var app = builder.Build();

//添加認證及授權中間件


// Configure the HTTP request pipeline.

app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
});
//app.MapGet("/", () => "Hello World!");


//允許跨域訪問資源
app.UseCors("ReactAppPolicy");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
