using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using mvc.Enums;
using mvc.Models.Authorize;

namespace mvc.Models
{
    public class Business
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string OwnerId { get; set; }  //fk from user identity
        [ForeignKey("OwnerId")]
        public virtual ApplicationUser Owner { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } 

        [Required]
        public int CategoryId { get; set; } //fk from cat table
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string MainImage { get; set; }

        //public List<string> Gallery { get; set; } //table Gallery (id,imgUrl,businessId)

        [Required]
        public string Latitude { get; set; }

        [Required]
        public string Longitude { get; set; }

        [Required]
        public string Address { get; set; }  

        //public List<string> Features { get; set; } //table  (id,describtion,bid)

        //========================================================

        //[Required]
        //public BusinessType BusinessType { get; set; } //enum //user can`t change it

        public bool IsActive { get; set; } //user can`t change it 

        public DateTime? SubscriptionEndDate { get; set; }  //user can`t change it

        // public List<MenuItem> MenuItems { get; set; }

        [ForeignKey("Package")]
        public int PackageId { get; set; }
        public Package Package { get; set; } 


        public  ICollection<Review>? Reviews { get; set; }
        public  ICollection<OpeningHour>? OpeningHours { get; set; }
        public  ICollection<Ad>? Advertisements { get; set; }

        
    }

  
}
