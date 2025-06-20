using Microsoft.AspNetCore.Identity; // IdentityUser sınıfını kullanabilmek için bu using direktifi gerekli
using System.Collections.Generic;     // ICollection<T> için bu using direktifi gerekli

namespace MyDiaryApp.Models
{
    // User sınıfımız, ASP.NET Core Identity'nin temel kullanıcı sınıfı olan IdentityUser'dan miras alıyor.
    // IdentityUser sınıfı zaten Id (string tipinde), UserName, Email, PasswordHash, PhoneNumber gibi
    // birçok standart kullanıcı özelliğini içerir. Bu yüzden bizim bu özellikleri tekrar tanımlamamıza gerek yok.
    // Sadece User'a özel eklemek istediğimiz ekstra özellikler olursa onları buraya ekleriz.
    public class User : IdentityUser
    {
        // Bir kullanıcının birden fazla günlük girişi olabilir.
        // Bu, User ve Entry arasında bire-çok bir ilişki olduğunu gösteren bir "navigation property"dir.
        // Başlangıçta boş bir liste olarak başlatıyoruz ki null referans hatası almayalım.
        public ICollection<Entry> Entries { get; set; } = new List<Entry>();

        // ÖRNEK: Eğer kullanıcı için bir "Doğum Tarihi" gibi özel bir alan eklemek isteseydik,
        // onu buraya ekleyebilirdik:
        // public DateTime? DateOfBirth { get; set; }
        // Ama şimdilik projemiz için böyle bir şeye ihtiyacımız yok.
    }
}