using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using mvc.Enums;

namespace mvc.Models
{
    public class Business
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string OwnerId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string MainImage { get; set; }

        public List<string> Gallery { get; set; }

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        [Required]
        public string Address { get; set; }

        public List<string> Features { get; set; }

        [Required]
        public BusinessType BusinessType { get; set; }

        public bool IsActive { get; set; }

        public DateTime? SubscriptionEndDate { get; set; }

       // public List<MenuItem> MenuItems { get; set; }

        //// Navigation Properties
        //[ForeignKey("OwnerId")]
        //public virtual User Owner { get; set; }

        [ForeignKey("CategoryId")]
        public  Category? Category { get; set; }

        public  ICollection<Review>? Reviews { get; set; }
        public  ICollection<OpeningHour>? OpeningHours { get; set; }
        public  ICollection<Ad>? Advertisements { get; set; }

        
    }

  
}
