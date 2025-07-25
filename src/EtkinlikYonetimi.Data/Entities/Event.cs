using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtkinlikYonetimi.Data.Entities
{
    /// <summary>
    /// Represents an event in the event management system
    /// </summary>
    public class Event
    {
        /// <summary>
        /// Gets or sets the unique identifier for the event
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the event title
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets when the event starts
        /// </summary>
        [Required]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets when the event ends
        /// </summary>
        [Required]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Gets or sets the path to the event's image
        /// </summary>
        [MaxLength(500)]
        public string? Image { get; set; }

        /// <summary>
        /// Gets or sets a short description of the event
        /// </summary>
        [MaxLength(512)]
        public string? ShortDescription { get; set; }

        /// <summary>
        /// Gets or sets a detailed description of the event
        /// </summary>
        public string? LongDescription { get; set; }

        /// <summary>
        /// Gets or sets whether the event is active and visible
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Gets or sets the ID of the user who created this event
        /// </summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets when the event record was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the user who created this event
        /// </summary>
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
    }
}