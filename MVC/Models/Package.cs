using System.ComponentModel.DataAnnotations;

namespace mvc.Models
{
    public class Package
    {
        [Key]
        public int Id { get; set; }   // 1 , 2 , 3 , 4

        [Required]
        public string Name { get; set; } // sponsared , Featured , Basic , ads

        [Required]
        public decimal monthlyPrice { get; set; } // 40$ , 20$ , 0$ , 35$

        [Required]
        public decimal yearlyPrice { get; set; }

        public bool IsActive { get; set; }
        //public Package()
        //{
        //    FeaturesIncluded = new List<string>();
        //    IsActive = true;
        //    MaxBusinessCount = 1;
        //    MaxImagesPerBusiness = 5;
        //    IncludesFeaturedListing = false;
        //    IncludesAdvertising = false;
        //    AdDaysIncluded = 0;
        //    MonthlyDiscountPercentage = 0;
        //}
    }
}
