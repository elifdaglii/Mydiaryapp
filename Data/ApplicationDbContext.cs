using Microsoft.EntityFrameworkCore; // DbContext ve DbSet için gerekli
using MyDiaryApp.Models; // User ve Entry sınıflarımız için

namespace MyDiaryApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        // Yapıcı (Constructor) - DbContextOptions alır ve base class'a (DbContext) iletir
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Veritabanında "Users" adında bir tablo oluşturacak ve User modelimizle eşleşecek
        public DbSet<User> Users { get; set; }

        // Veritabanında "Entries" adında bir tablo oluşturacak ve Entry modelimizle eşleşecek
        public DbSet<Entry> Entries { get; set; }

        // (Opsiyonel ama iyi bir pratik) Model oluşturulurken ek yapılandırmalar için
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User ve Entry arasındaki ilişkiyi burada daha detaylı yapılandırabiliriz,
            // ama EF Core çoğu basit ilişkiyi zaten otomatik olarak anlar.
            // Örneğin, bir kullanıcının birden çok günlüğü olabilir, bir günlük bir kullanıcıya aittir.
            // Bu, User ve Entry sınıflarındaki navigation property'ler ve ForeignKey ile zaten belirtildi.

            // Örnek: Email adresinin benzersiz (unique) olmasını sağlamak için:
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}