using System.ComponentModel.DataAnnotations;

namespace EtkinlikYonetimi.Business.DTOs
{
    public class UserLoginDto
    {
        [Required(ErrorMessage = "Email adresi gereklidir.")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
        [Display(Name = "Email Adresi")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Parola gereklidir.")]
        [Display(Name = "Parola")]
        public string Password { get; set; } = string.Empty;
    }
}