using BCrypt.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using VitaLink.Models.Data; 
using Vitalink.API.Hubs; 
using Vitalink.Models;
using Vitalink.API.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. إعداد المتحكمات مع معالجة التكرار في JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// 2. إعداد قاعدة البيانات
builder.Services.AddDbContextFactory<VitalinkDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        }));

// 3. تصحيح كود الـ CORS (هنا كان الخطأ)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.SetIsOriginAllowed(origin => 
                new Uri(origin).Host.EndsWith("vercel.app") || 
                origin == "http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// 4. تسجيل الخدمات (Dependency Injection)
builder.Services.AddScoped<ISensorDataService, SensorDataService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddSingleton<ConnectionTracker>();
builder.Services.AddSignalR();

// 5. إعداد Swagger و Auth
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // تأكد أن هذه القيم موجودة في appsettings.json
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

var app = builder.Build();

// --- ترتيب الـ Middleware (مهم جداً) ---

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// يجب أن يكون الـ CORS قبل الـ Auth والـ Map
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<SensorDataHub>("/sensorhub");

app.Run();
