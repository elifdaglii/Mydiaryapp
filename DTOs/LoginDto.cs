using System.ComponentModel.DataAnnotations; // [Required], [EmailAddress] için

namespace MyDiaryApp.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "E-posta adresi gereklidir.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string Email { get; set; } = null!; // CS8618 uyarısı için null forgiving

        [Required(ErrorMessage = "Şifre gereklidir.")]
        public string Password { get; set; } = null!; // CS8618 uyarısı için null forgiving
    }
}