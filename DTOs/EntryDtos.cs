using System.ComponentModel.DataAnnotations;

namespace MyDiaryApp.DTOs
{
    public class CreateEntryDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title must be between 1 and 200 characters", MinimumLength = 1)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Content is required")]
        [StringLength(5000, ErrorMessage = "Content must be between 1 and 5000 characters", MinimumLength = 1)]
        public string Content { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Mood must be less than 50 characters")]
        public string? Mood { get; set; }
    }

    public class UpdateEntryDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title must be between 1 and 200 characters", MinimumLength = 1)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Content is required")]
        [StringLength(5000, ErrorMessage = "Content must be between 1 and 5000 characters", MinimumLength = 1)]
        public string Content { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Mood must be less than 50 characters")]
        public string? Mood { get; set; }
    }

    public class EntryDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? Mood { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}