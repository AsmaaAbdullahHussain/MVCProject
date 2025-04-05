using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace mvc.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        [ForeignKey("Business")]
        public int BusinessId { get; set; }

        [Required, Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        public string Comment { get; set; }

        public DateTime CreatedAt { get; set; }
        public Business Business { get; set; }
        public bool IsRead { get; set; }

        public Review()
        {
            CreatedAt = DateTime.UtcNow;
        }
    }
}