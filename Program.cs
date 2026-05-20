using ApiFinanceiro.DataContexts;
using ApiFinanceiro.Profiles;
using ApiFinanceiro.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("mysql");
builder.Services.AddDbContext<AppDbContext>(
    options => options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 32)))
 );



builder.Services.AddControllers().AddJsonOptions(
    options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<DespesaService>();
builder.Services.AddScoped<PasswordService>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddAutoMapper(config => config.AddProfile<DespesaProfile>());

var jwtKey = builder.Configuration["Jwt:Key"] ?? "eventhub-dev-secret-key-change-me-32";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "EventHub";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "EventHub";

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
