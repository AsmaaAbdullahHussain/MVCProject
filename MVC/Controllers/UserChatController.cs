using Microsoft.AspNetCore.Mvc;

namespace mvc.Controllers
{
    public class UserChatController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
