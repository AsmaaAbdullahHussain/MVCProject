namespace mvc.ViewModels.PackageVM
{
    public class UpdatePackageVM
    {
        public string Name { get; set; }
        public decimal MonthlyPrice { get; set; }
        public decimal YearlyPrice { get; set; }
        public bool IsActive { get; set; }
    }
}
