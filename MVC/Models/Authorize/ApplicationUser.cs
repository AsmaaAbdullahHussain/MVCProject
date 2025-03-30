using Microsoft.AspNetCore.Identity;

namespace mvc.Models.Authorize
{
    public class ApplicationUser:IdentityUser //by default <string> 
    {
        public string Address { get; set; }
    }
}
