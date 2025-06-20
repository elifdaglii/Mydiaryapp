using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MyDiaryApp.Models; // User için bu using gerekli olabilir, kontrol et

namespace MyDiaryApp.Models
{
    public class Entry
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = null!; // EKLENDİ

        [Required]
        public string Content { get; set; } = null!; // EKLENDİ

        [MaxLength(50)]
        public string? Mood { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; } = null!; // EKLENDİ
    }
}