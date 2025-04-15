using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mvc.Models
{
    public class Package
    {
        public Package()
        {
            Features = new List<PackageFeature>();
        }

        [Key]
        public int Id { get; set; }   

        [Required]
        [StringLength(100)]
        public string Name { get; set; } // sponsared , Featured , Basic 

        [Required]
        public decimal MonthlyPrice { get; set; } 

        [Required]
        public decimal YearlyPrice { get; set; }

        public string Description { get; set; }

        // الخصائص
        public virtual ICollection<PackageFeature> Features { get; set; }
        public ICollection<Business>? Businesses { get; set; } 
    }
}
