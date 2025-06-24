using Microsoft.AspNetCore.Authorization; // [Authorize] attribute'u için
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;     // Include, ToListAsync gibi EF metotları için
using MyDiaryApp.Data;                 // ApplicationDbContext için
using MyDiaryApp.DTOs;                 // EntryDto, CreateEntryDto, UpdateEntryDto için
using MyDiaryApp.Models;               // Entry, User modelleri için
using System.Linq;                     // FirstOrDefaultAsync, Where gibi LINQ metotları için
using System.Security.Claims;          // User.FindFirstValue(ClaimTypes.NameIdentifier) için
using System.Threading.Tasks;          // Task için
using System.Collections.Generic;      // List<T> için

namespace MyDiaryApp.Controllers
{
    [Authorize] // BU ÇOK ÖNEMLİ! Bu controller'daki tüm endpoint'lere sadece giriş yapmış kullanıcılar erişebilir.
    [ApiController]
    [Route("api/[controller]")] // API yolu /api/entries olacak
    public class EntryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public EntryController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/entries (Giriş yapmış kullanıcının tüm günlüklerini getirir)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EntryDto>>> GetUserEntries()
        {
            // Şu anki giriş yapmış kullanıcının Id'sini al
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized(new { Message = "Kullanıcı bulunamadı veya giriş yapılmamış." });
            }

            var entries = await _context.Entries
                                    .Where(e => e.UserId == userId) // Sadece bu kullanıcıya ait günlükleri seç
                                    .OrderByDescending(e => e.CreatedAt) // En yeniden eskiye sırala
                                    .Select(e => new EntryDto // EntryDto'ya dönüştür
                                    {
                                        Id = e.Id,
                                        Title = e.Title,
                                        Content = e.Content,
                                        Mood = e.Mood,
                                        CreatedAt = e.CreatedAt,
                                        UpdatedAt = e.UpdatedAt
                                    })
                                    .ToListAsync();

            return Ok(entries);
        }

        // GET: api/entries/{id} (Belirli bir günlüğü Id ile getirir)
        [HttpGet("{id}")]
        public async Task<ActionResult<EntryDto>> GetEntry(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var entry = await _context.Entries
                                .Where(e => e.UserId == userId && e.Id == id) // Sadece bu kullanıcıya ait ve Id'si eşleşen
                                .Select(e => new EntryDto
                                {
                                    Id = e.Id,
                                    Title = e.Title,
                                    Content = e.Content,
                                    Mood = e.Mood,
                                    CreatedAt = e.CreatedAt,
                                    UpdatedAt = e.UpdatedAt
                                })
                                .FirstOrDefaultAsync();

            if (entry == null)
            {
                return NotFound(new { Message = "Günlük bulunamadı veya bu günlüğe erişim yetkiniz yok." });
            }

            return Ok(entry);
        }

        // POST: api/entries (Yeni bir günlük oluşturur)
        [HttpPost]
        public async Task<ActionResult<EntryDto>> CreateEntry(CreateEntryDto createEntryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized(new { Message = "Yeni günlük oluşturmak için giriş yapmalısınız." });
            }

            var entry = new Entry
            {
                Title = createEntryDto.Title,
                Content = createEntryDto.Content,
                Mood = createEntryDto.Mood,
                CreatedAt = DateTime.UtcNow,
                UserId = userId // Günlüğü oluşturan kullanıcının Id'si
            };

            _context.Entries.Add(entry);
            await _context.SaveChangesAsync();

            // Oluşturulan kaynağı ve kaynağın yolunu döndürmek iyi bir pratiktir (201 Created ile)
            // Ama önce EntryDto'ya dönüştürmemiz lazım
            var entryDto = new EntryDto
            {
                Id = entry.Id,
                Title = entry.Title,
                Content = entry.Content,
                Mood = entry.Mood,
                CreatedAt = entry.CreatedAt,
                UpdatedAt = entry.UpdatedAt
            };

            return CreatedAtAction(nameof(GetEntry), new { id = entry.Id }, entryDto);
        }

        // PUT: api/entries/{id} (Belirli bir günlüğü günceller)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEntry(int id, UpdateEntryDto updateEntryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var entry = await _context.Entries.FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

            if (entry == null)
            {
                return NotFound(new { Message = "Günlük bulunamadı veya bu günlüğü güncelleme yetkiniz yok." });
            }

            entry.Title = updateEntryDto.Title;
            entry.Content = updateEntryDto.Content;
            entry.Mood = updateEntryDto.Mood;
            entry.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Eğer aynı anda başka birisi bu kaydı değiştirdiyse veya sildiyse bu hata alınabilir.
                // Bu durumu ele almak için daha karmaşık mantıklar eklenebilir.
                // Şimdilik basitçe NotFound döndürelim.
                if (!_context.Entries.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw; // Başka bir concurrency sorunu varsa hatayı olduğu gibi fırlat
                }
            }

            return NoContent(); // Başarılı güncellemede genellikle 204 No Content döndürülür
        }
[AllowAnonymous] // BU ÖNEMLİ: Bu endpoint için yetkilendirme GEREKMESİN
    [HttpGet("test")] // Yolu /api/entries/test olacak
    public IActionResult TestEndpoint()
    {
        return Ok("EntryController test endpoint basariyla calisti!");
    }
        // DELETE: api/entries/{id} (Belirli bir günlüğü siler)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEntry(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var entry = await _context.Entries.FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
            if (entry == null)
            {
                return NotFound(new { Message = "Günlük bulunamadı veya bu günlüğü silme yetkiniz yok." });
            }

            _context.Entries.Remove(entry);
            await _context.SaveChangesAsync();

            return NoContent(); // Başarılı silmede genellikle 204 No Content döndürülür
        }
    }
}