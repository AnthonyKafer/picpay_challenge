using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using picpay_challenge.Domain.Data;
using picpay_challenge.Domain.Integrations;
using picpay_challenge.Domain.Repositories;
using picpay_challenge.Domain.Services;
using picpay_challenge.Repositories.picpay_challenge.Repositories;
using System.Text;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"));
});
var key = builder.Configuration["Jwt:key"];
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    var isDev = Environment.GetEnvironmentVariable("IsDevelopment")?.ToLower() == "true";
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = isDev,
        ValidateAudience = isDev,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
    };
});
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.NumberHandling =
          System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString;
});
builder.Services.AddAuthorization();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<TransactionRepository>();
builder.Services.AddScoped<TransactionService>();
builder.Services.AddSingleton<AuthService>();
builder.Services.AddHttpClient<PaymentExternalAuthorizor>();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
