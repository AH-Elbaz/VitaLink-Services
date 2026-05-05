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

// 1. إعداد المتحكمات (مرة واحدة فقط)
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// 2. إعداد CORS للسماح لأي مصدر في العالم
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.SetIsOriginAllowed(_ => true) // يسمح بأي Origin مهما كان
              .AllowAnyMethod()              // يسمح بجميع الأفعال (GET, POST, etc.)
              .AllowAnyHeader()              // يسمح بجميع الـ Headers
              .AllowCredentials();           // ضروري جداً لعمل SignalR والـ Auth
    });
});

// 3. قاعدة البيانات
builder.Services.AddDbContextFactory<VitalinkDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
        }));

// 4. تسجيل الخدمات
builder.Services.AddScoped<ISensorDataService, SensorDataService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddSingleton<ConnectionTracker>();
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 5. إعداد المصادقة (Authentication)
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "YourDefaultFallbackKeyForDevelopment"))
    };
});

var app = builder.Build();

// --- ترتيب الـ Middleware (حرج جداً) ---

app.UseSwagger();
app.UseSwaggerUI();

// 1. يجب أن يكون الـ CORS أول شيء تقريباً
app.UseCors("AllowAllOrigins");

app.UseHttpsRedirection();

// 2. المصادقة يجب أن تأتي بعد الـ CORS وقبل الـ Map
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<SensorDataHub>("/sensorhub");

app.Run();
