using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace mvc.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public int BusinessId { get; set; }

        [Required, Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        public string Comment { get; set; }

        public DateTime CreatedAt { get; set; }

        //// Navigation Properties
        //[ForeignKey("UserId")]
        //public virtual User User { get; set; }

        [ForeignKey("BusinessId")]
        public Business? Business { get; set; }

        public Review()
        {
            CreatedAt = DateTime.UtcNow;
        }
    }
}