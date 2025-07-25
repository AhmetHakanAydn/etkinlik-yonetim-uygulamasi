using System;
using System.ComponentModel.DataAnnotations;

namespace EtkinlikYonetimi.Business.DTOs
{
    public class UserRegisterDto
    {
        [Required(ErrorMessage = "Email adresi gereklidir.")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
        [Display(Name = "Email Adresi")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Parola gereklidir.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Parola en az 8 karakter olmalıdır.")]
        [Display(Name = "Parola")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Parola tekrarı gereklidir.")]
        [Compare("Password", ErrorMessage = "Parolalar eşleşmiyor.")]
        [Display(Name = "Parola Tekrar")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ad gereklidir.")]
        [StringLength(100, ErrorMessage = "Ad en fazla 100 karakter olabilir.")]
        [Display(Name = "Ad")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad gereklidir.")]
        [StringLength(100, ErrorMessage = "Soyad en fazla 100 karakter olabilir.")]
        [Display(Name = "Soyad")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Doğum tarihi gereklidir.")]
        [DataType(DataType.Date)]
        [Display(Name = "Doğum Tarihi")]
        public DateTime BirthDate { get; set; }
    }
}