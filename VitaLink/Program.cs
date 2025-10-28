using BCrypt.Net; // يجب استيراد BCrypt بشكل صحيح
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Text; // لاستخدام النماذج
using Vitalink.API.Services;
using VitaLink.Models.Data;
// --------------------------------------------------------------------------------------
// ملاحظة: تم إزالة دالة GenerateCorrectHash() من هنا ونقلها إلى Service أو Controller
// لمنع أخطاء التجميع/البناء (Compilation Errors)، حيث لا يمكن وضعها في هذا المكان.
// --------------------------------------------------------------------------------------

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

// 1.3. تكوين CORS (السماح لأي نطاق في بيئة التطوير)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            // تحذير: هذا الخيار غير آمن للإنتاج، لكنه يحل مشكلة CORS مؤقتاً.
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// 1.4. تكوين خدمة مصادقة JWT
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

app.MapControllers();

app.Run();