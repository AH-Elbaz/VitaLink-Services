using BCrypt.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR; // لخدمات SignalR
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; // ILogger
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Text;
using VitaLink.Models.Data; // تأكد من أن هذا هو مسار DbContext الصحيح
using Vitalink.API.Hubs; // يجب استيراد Hubs هنا 
using Vitalink.Models; // لاستخدام النماذج
using Vitalink.API.Services;

var builder = WebApplication.CreateBuilder(args);

// --------------------------------------------------------------------------------------
// 1. SERVICES CONFIGURATION
// --------------------------------------------------------------------------------------

// 1.1. تسجيل DbContext وتفعيل مرونة الأخطاء العابرة (Retry Policy)
builder.Services.AddDbContext<VitalinkDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptionsAction: sqlOptions =>
        {
            // تمكين مرونة الأخطاء العابرة (للاتصال بـ Azure SQL)
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        }));

// 1.2. تسجيل خدمة التوكن (JWT Service)
builder.Services.AddScoped<ITokenService, TokenService>();

// ** 1.3. تسجيل خدمة SignalR Hubs **
builder.Services.AddSignalR();


// 1.4. تكوين CORS (للسماح بالوصول العام الآمن)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", // اسم السياسة التي تستخدمها
        builder =>
        {
            // الحل النهائي لـ CORS: SetIsOriginAllowed يسمح بالوصول العام ويحل مشكلة التعارض مع AllowCredentials
            builder.SetIsOriginAllowed(origin => true)
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials(); // ضروري لـ SignalR وتمرير التوكنات
        });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// 1.5. تكوين خدمة مصادقة JWT
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});


// --------------------------------------------------------------------------------------
// 2. HTTP REQUEST PIPELINE (MIDDLEWARE)
// --------------------------------------------------------------------------------------

var app = builder.Build();

// 2.1. تفعيل CORS أولاً (قبل المصادقة)
app.UseCors("AllowAllOrigins");

// 2.2. تفعيل Swagger (لأغراض الاختبار في Azure)
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// 2.3. تفعيل المصادقة (Authentication)
app.UseAuthentication();
app.UseAuthorization();

// 2.4. تعيين مسارات الـ Hubs والـ Controllers
app.MapControllers();
app.MapHub<SensorDataHub>("/sensorhub"); // <--- المسار المطلوب لـ WebSocket Hub

app.Run();