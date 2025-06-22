using System.Text; // Encoding.UTF8.GetBytes için
using Microsoft.AspNetCore.Authentication.JwtBearer; // JwtBearerDefaults için
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyDiaryApp.Data;
using MyDiaryApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Veritabanı bağlantısını ekle (DbContext)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ASP.NET Core Identity servislerini ekle
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    // İsteğe bağlı: Şifre ayarları
    options.Password.RequireDigit = true;       // Şifrede rakam olsun mu?
    options.Password.RequireLowercase = true;   // Küçük harf olsun mu?
    options.Password.RequireUppercase = true;   // Büyük harf olsun mu?
    options.Password.RequireNonAlphanumeric = false; // Özel karakter (@,!,# vb.) olsun mu?
    options.Password.RequiredLength = 6;        // Minimum şifre uzunluğu
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders(); // Şifre sıfırlama gibi işlemler için token üreticilerini ekler
// JWT Authentication/Authorization Ayarları
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true; // Token'ı HttpContext'te sakla
    options.RequireHttpsMetadata = false; // Geliştirme ortamında HTTPS zorunluluğunu kaldırabiliriz, production'da true olmalı
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true, // Yayıncıyı doğrula
        ValidateAudience = true, // Hedef kitleyi doğrula
        ValidateLifetime = true, // Token'ın ömrünü doğrula
        ValidateIssuerSigningKey = true, // İmza anahtarını doğrula
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"], // appsettings.json'dan al
        ValidAudience = builder.Configuration["JwtSettings:Audience"], // appsettings.json'dan al
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!)) // appsettings.json'dan al
        // ClockSkew = TimeSpan.Zero // Token süresinin dolmasına ne kadar tolerans tanınacağı (varsayılan 5Har dk)
    };
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // Önce kimlik doğrulama
app.UseAuthorization();  // Sonra yetkilendirme

app.MapControllers();

app.Run();