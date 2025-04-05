using mvc.Enums;
using mvc.Models.Authorize;
using mvc.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace mvc.ViewModels
{
    public class BusinessViewModel
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; }

        //[Required]
        //public int CategoryId { get; set; } 

        [Required]
        public string Description { get; set; }

        [Required]
        public string MainImage { get; set; }

        [Required]
        public string Latitude { get; set; }

        [Required]
        public string Longitude { get; set; }
        [Required]
        public string Address { get; set; }
        public bool IsActive { get; set; } = false; //user can`t change it
        public DateTime? SubscriptionEndDate { get; set; }
        public List<Category>? Category { get; set; }

        public List<BusinessFeatures>? BusinessFeatures { get; set; }

        public List<string>? businessesNameList { get; set; }

        public int CategoryId { get; set; }
        
        public List<Category>? categories { get; set; }
    }
}
