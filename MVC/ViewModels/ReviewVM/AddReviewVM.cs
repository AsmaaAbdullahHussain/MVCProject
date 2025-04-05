using System.ComponentModel.DataAnnotations;

namespace mvc.ViewModels.ReviewVM
{
    public class AddReviewVM
    {
        [EmailAddress]
        public string Email { get; set; }
        public int BusinessId { get; set; }
        [Range(0,5)]
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}
