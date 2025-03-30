using mvc.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace mvc.Models
{
    public class Ad
    {
        [Key]
        public int Id { get; set; }

        public int? BusinessId { get; set; }

        [Required]
        public string Image { get; set; }

        [Required]
        public string Link { get; set; }

        [Required]
        public AdType Type { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public PackageType PackageType { get; set; }

        public bool IsActive { get; set; }

        // Navigation Properties
        [ForeignKey("BusinessId")]
        public Business? Business { get; set; }

        public Ad()
        {
            StartDate = DateTime.UtcNow;
            IsActive = true;
        }
    }
}