using mvc.Attributes;
using mvc.Models.CategoryValidation;

namespace mvc.ViewModels.PackageVM
{
    public class UpdatePackageVM
    {
         public int Id { get; set; }   
        public string Name { get; set; } // sponsared , Featured , Basic 
        public decimal MonthlyPrice { get; set; } 
        public decimal YearlyPrice { get; set; }
        public string Description { get; set; }

    }
}
