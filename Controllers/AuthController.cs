using Microsoft.AspNetCore.Identity; // UserManager, SignInManager, IdentityUser için
using Microsoft.AspNetCore.Mvc;     // ControllerBase, IActionResult, HttpPost, Route için
using MyDiaryApp.DTOs;             // RegisterDto için
using MyDiaryApp.Models;           // User modelimiz için
using System.Threading.Tasks;      // Task için

namespace MyDiaryApp.Controllers
{
    [ApiController] // Bu sınıfın bir API controller olduğunu belirtir
    [Route("api/[controller]")] // API endpoint'inin yolu: /api/auth olacak
    public class AuthController : ControllerBase // API controller'ları için temel sınıf
    {
        private readonly UserManager<User> _userManager;
        // private readonly SignInManager<User> _signInManager; // Login için sonra ekleyeceğiz
        // private readonly IConfiguration _configuration; // Token oluşturma için sonra ekleyeceğiz

        // Constructor (Yapıcı Metot) - Gerekli servisleri enjekte ediyoruz
        public AuthController(UserManager<User> userManager) // SignInManager ve IConfiguration'ı sonra ekleyeceğiz
        {
            _userManager = userManager;
            // _signInManager = signInManager;
            // _configuration = configuration;
        }

        // POST api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            // Gelen verinin geçerli olup olmadığını kontrol et (DTO'daki Data Annotations sayesinde)
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Geçersizse 400 Bad Request ve hataları döndür
            }

            // Yeni bir User nesnesi oluştur
            var user = new User
            {
                UserName = registerDto.Username, // IdentityUser'da UserName zorunludur
                Email = registerDto.Email
                // PasswordHash, UserManager tarafından CreateAsync içinde otomatik olarak oluşturulacak
            };

            // UserManager'ı kullanarak kullanıcıyı şifresiyle birlikte oluştur
            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (result.Succeeded)
            {
                // Kullanıcı başarıyla oluşturuldu
                // Şimdilik basit bir mesaj döndürelim.
                // İleride oluşturulan kullanıcıyı veya bir token'ı döndürebiliriz.
                return StatusCode(201, new { Message = "Kullanıcı başarıyla oluşturuldu." });
            }

            // Eğer kullanıcı oluşturma başarısız olduysa, hataları döndür
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return BadRequest(ModelState);
        }

        // Login endpoint'i buraya eklenecek (sonraki adımda)
    }
}