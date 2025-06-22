using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyDiaryApp.DTOs;
using MyDiaryApp.Models;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration; // Bu using'i eklemiştin, doğru
using Microsoft.IdentityModel.Tokens;    // Bu using'i eklemiştin, doğru
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyDiaryApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager; // Artık yorum değil
        private readonly IConfiguration _configuration;     // Artık yorum değil

        // Constructor (Yapıcı Metot) - GÜNCELLENDİ
        public AuthController(
            UserManager<User> userManager,
            SignInManager<User> signInManager, // PARAMETRE EKLENDİ
            IConfiguration configuration)      // PARAMETRE EKLENDİ
        {
            _userManager = userManager;
            _signInManager = signInManager;     // ATAMA YAPILDI
            _configuration = configuration;     // ATAMA YAPILDI
        }

        // POST api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new User
            {
                UserName = registerDto.Username,
                Email = registerDto.Email
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (result.Succeeded)
            {
                return StatusCode(201, new { Message = "Kullanıcı başarıyla oluşturuldu." });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return BadRequest(ModelState);
        }

        // POST api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return Unauthorized(new { Message = "Geçersiz e-posta veya şifre." });
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var tokenString = GenerateJwtToken(user);
                return Ok(new { Token = tokenString, Message = "Giriş başarılı." });
            }

            return Unauthorized(new { Message = "Geçersiz e-posta veya şifre." });
        }

        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("uid", user.Id)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    } // AuthController sınıfının kapanışı
} 