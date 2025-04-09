using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using System.Linq;
using mvc.Models.Authorize;
using mvc.Models;
using mvc.ViewModels; // Make sure this namespace is included
using MVC.Models;

namespace mvc.Controllers
{
    public class UserChatController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ProjectContext _context;

        public UserChatController(UserManager<ApplicationUser> userManager, ProjectContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        
        [HttpGet]
        public async Task<IActionResult> GetUserInfo()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            
            return Json(new { userId, userName = user.UserName });
        }
        
        [HttpGet]
        public async Task<IActionResult> GetPreviousMessages()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            
            // Fixed the query to use the correct properties and handle possible null values
            var messages = await _context.ChatMessages
                .Where(m => m.UserId == userId || (m.IsFromAdmin && m.RecipientId == userId))
                .OrderBy(m => m.SentAt)
                .Select(m => new ChatMessageViewModel
                {
                    Id = m.Id,
                    Content = m.Content,
                    SenderId = m.IsFromAdmin ? "admin" : m.SenderId ?? m.UserId, // Use SenderId if available, fallback to UserId
                    SenderName = m.IsFromAdmin ? "الدعم الفني" : m.SenderName ?? "Unknown User",
                    SentAt = m.SentAt,
                    IsRead = m.IsRead,
                    ConversationId = m.ConversationId
                })
                .ToListAsync();
            
            return Json(messages);
        }
    }
}
