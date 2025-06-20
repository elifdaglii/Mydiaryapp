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