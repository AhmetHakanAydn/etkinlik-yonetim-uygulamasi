using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace EtkinlikYonetimi.Business.DTOs
{
    public class EventDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Başlık gereklidir.")]
        [StringLength(255, ErrorMessage = "Başlık en fazla 255 karakter olabilir.")]
        [Display(Name = "Başlık")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Başlangıç tarihi gereklidir.")]
        [Display(Name = "Başlangıç Tarihi")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Bitiş tarihi gereklidir.")]
        [Display(Name = "Bitiş Tarihi")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Resim")]
        public string? Image { get; set; }

        [StringLength(512, ErrorMessage = "Kısa açıklama en fazla 512 karakter olabilir.")]
        [Display(Name = "Kısa Açıklama")]
        public string? ShortDescription { get; set; }

        [Display(Name = "Uzun Açıklama")]
        public string? LongDescription { get; set; }

        [Display(Name = "Aktif")]
        public bool IsActive { get; set; } = true;

        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public UserDto? User { get; set; }

        // For file upload
        [Display(Name = "Resim Dosyası")]
        public IFormFile? ImageFile { get; set; }
    }
}