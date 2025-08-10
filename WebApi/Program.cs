using BusinessLogic.Data;
using BusinessLogic.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using WebApi.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Net.Http.Headers;
using BusinessLogic.Repositories;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;


var builder = WebApplication.CreateBuilder(args);

// Configuramos Serilog 
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Agregamos JWT de autenticación
var jwtConfig = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtConfig["Issuer"],
        ValidAudience = jwtConfig["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["Key"]))
    };
});

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IParameterService, ParameterService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserSeriesService, UserSeriesService>();

builder.Services.AddScoped<ISapSessionService, SapSessionService>();

builder.Services.AddScoped<ISalesPersonRepository, SalesPersonRepository>();
builder.Services.AddScoped<ISalesPersonService, SalesPersonService>();

builder.Services.AddScoped<ISalesQuotationRepository, SalesQuotationRepository>();
builder.Services.AddScoped<ISalesQuotationService, SalesQuotationService>();

builder.Services.AddScoped<ISalesOrderRepository, SalesOrderRepository>();
builder.Services.AddScoped<ISalesOrderService, SalesOrderService>();

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddScoped<IItemService, ItemService>();

builder.Services.AddScoped<IUnitOfMeasureRepository, UnitOfMeasureRepository>();
builder.Services.AddScoped<IUnitOfMeasureService, UnitOfMeasureService>();

builder.Services.AddScoped<ITermsConditionsRepository, TermsConditionsRepository>();
builder.Services.AddScoped<ITermsConditionsService, TermsConditionsService>();

builder.Services.AddScoped<IPdfReportService, PdfReportService>();
// Para la cadena de conexión a la base de datos
builder.Services.AddScoped<IConnectionStringService, ConnectionStringService>();

builder.Services.AddScoped<IJwtService, JwtService>();


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<WebApi.MiddleWare.ErrorHandlingMiddleWare>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
