using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public virtual Business Business { get; set; }

        public Ad()
        {
            StartDate = DateTime.UtcNow;
            IsActive = true;
        }
    }

    public enum AdType
    {
        BusinessAd,
        ExternalAd
    }

    public enum PackageType
    {

        Weekly,
        Monthly
    }
} 