using System.ComponentModel.DataAnnotations;

namespace EtkinlikYonetimi.Data.Entities
{
    /// <summary>
    /// Represents a user in the event management system
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets or sets the unique identifier for the user
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the user's email address
        /// </summary>
        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's encrypted password
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's first name
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's last name
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's birth date
        /// </summary>
        [Required]
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// Gets or sets when the user account was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the collection of events created by this user
        /// </summary>
        public virtual ICollection<Event> Events { get; set; } = new List<Event>();
    }
}