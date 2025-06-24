using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MyDiaryApp.Models
{
    public class User : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        // Navigation property - bir kullanıcının birden fazla günlük kaydı olabilir
        public virtual ICollection<Entry> Entries { get; set; } = new List<Entry>();
    }
}