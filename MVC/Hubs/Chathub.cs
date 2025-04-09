using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using mvc.Models.Authorize;
using Microsoft.AspNetCore.Identity;
using mvc.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System;
using MVC.Models;
using MVC.ViewModels;

namespace mvc.Hubs
{
    public class Chathub : Hub
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ProjectContext _context;

        public Chathub(
            UserManager<ApplicationUser> userManager,
            ProjectContext context)
        {
            _userManager = userManager;
            _context = context;
        }


        // الحصول على محادثات الأدمن مع تحسين الأداء
        public async Task<List<AdminConversationViewModel>> GetAdminConversations()

        {
            var currentUserId = Context.UserIdentifier;
            var user = await _userManager.FindByIdAsync(currentUserId);

            if (user == null || !await _userManager.IsInRoleAsync(user, "Admin"))
                return new List<AdminConversationViewModel>();

            return await _context.Conversations
                .Where(c => c.IsAdminBroadcast || c.AdminId == currentUserId)
                .OrderByDescending(c => c.LastMessageAt)
                .Select(c => new AdminConversationViewModel
                {
                    Id = c.Id,
                    UserName = c.User.UserName,
                    UserId = c.UserId,
                    LastMessage = c.Messages.OrderByDescending(m => m.SentAt)
                                          .Select(m => m.Content)
                                          .FirstOrDefault(),
                    LastMessageAt = c.LastMessageAt ?? c.CreatedAt,
                    UnreadCount = c.Messages.Count(m => !m.IsRead && m.SenderId != currentUserId)
                })
              
                .ToListAsync();
        }


        public async Task<List<UserConversationDto>> GetUserConversations()
        {
            var currentUserId = Context.UserIdentifier;
            var user = await _userManager.FindByIdAsync(currentUserId);

            if (user == null) return new List<UserConversationDto>();

            return await _context.Conversations
                .Where(c => c.UserId == currentUserId)
                .OrderByDescending(c => c.LastMessageAt)
                .Select(c => new UserConversationDto
                {
                    Id = c.Id,
                    AdminName = c.Admin != null ? c.Admin.UserName : "الدعم الفني",
                    LastMessage = c.Messages
                                 .OrderByDescending(m => m.SentAt)
                                 .Select(m => m.Content)
                                 .FirstOrDefault(),
                    LastMessageAt = c.LastMessageAt ?? c.CreatedAt,
                    UnreadCount = c.Messages
                                 .Count(m => !m.IsRead &&
                                           m.SenderId != currentUserId &&
                                           m.SenderId == c.AdminId)
                })
              
                .ToListAsync();
        }














        public async Task<List<ChatMessageViewModel>> GetConversationMessages(string conversationId)
        {
            var currentUser = await _userManager.GetUserAsync(Context.User);
            if (currentUser == null) return new List<ChatMessageViewModel>();

            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");
            var hasAccess = await _context.Conversations
                .AnyAsync(c => c.Id == conversationId &&
                    (c.UserId == currentUser.Id || c.AdminId == currentUser.Id || isAdmin));

            if (!hasAccess) return new List<ChatMessageViewModel>();

            // تحديث حالة الرسائل كمقروءة
            //var unreadMessages = await _context.ChatMessages
            //    .Where(m => m.ConversationId == conversationId &&
            //               !m.IsRead &&
            //               m.SenderId != currentUser.Id)
            //    .ToListAsync();

            //unreadMessages.ForEach(m => m.IsRead = true);
            await _context.SaveChangesAsync();

            return await _context.ChatMessages
                .Where(m => m.ConversationId == conversationId)
                .OrderBy(m => m.SentAt)
                .Select(m => new ChatMessageViewModel
                {
                    Id = m.Id,
                    Content = m.Content,
                    SenderId = m.SenderId,
                    SenderName = m.Sender.UserName,
                    SentAt = m.SentAt,
                    IsRead = m.IsRead
                })
           //     .AsNoTracking()
                .ToListAsync();
        }

        // إرسال رسالة من المستخدم إلى الأدمنز
        public async Task SendToAdmins(string message)
        {
            var user = await _userManager.GetUserAsync(Context.User);
            if (user == null || string.IsNullOrWhiteSpace(message)) return;

            var conversation = new Conversation
            {
                UserId = user.Id,
                IsAdminBroadcast = true,
                CreatedAt = DateTime.UtcNow,
                LastMessageAt = DateTime.UtcNow
            };

            _context.Conversations.Add(conversation);
            await _context.SaveChangesAsync();

            var chatMessage = new ChatMessage
            {
                ConversationId = conversation.Id,
                SenderId = user.Id,
                Content = message.Trim(),
                SentAt = DateTime.UtcNow,
                IsRead = false
            };

            _context.ChatMessages.Add(chatMessage);
            await _context.SaveChangesAsync();

            await Clients.Group("Admins").SendAsync("ReceiveAdminMessage", new
            {
                ConversationId = conversation.Id,
                Message = message,
                SenderId = user.Id,
                SenderName = user.UserName,
                UserId = user.Id,
                Timestamp = DateTime.UtcNow
            });
        }

        // إرسال رد من الأدمن إلى المستخدم مع تحسينات
        public async Task SendReply(string conversationId, string message)
        {
            var admin = await _userManager.GetUserAsync(Context.User);
            if (admin == null || !await _userManager.IsInRoleAsync(admin, "Admin") ||
                string.IsNullOrWhiteSpace(message))
                return;

            var conversation = await _context.Conversations
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == conversationId);

            if (conversation == null) return;

            var chatMessage = new ChatMessage
            {
                ConversationId = conversationId,
                SenderId = admin.Id,
                Content = message.Trim(),
                SentAt = DateTime.UtcNow,
                IsRead = false
            };

            _context.ChatMessages.Add(chatMessage);
            conversation.LastMessageAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            await Clients.User(conversation.UserId).SendAsync("ReceiveReply", new
            {
                Message = message,
                AdminName = admin.UserName,
                Timestamp = DateTime.UtcNow,
                ConversationId = conversationId
            });

            // إعلام الأدمن الآخرين بالتحديث
            await Clients.Group("Admins").SendAsync("UpdateAdminConversation", new
            {
                ConversationId = conversationId,
                UserId = conversation.UserId,
                UserName = conversation.User.UserName,
                LastMessage = message,
                LastMessageAt = DateTime.UtcNow,
                AdminName = admin.UserName
            });
        }
        // إدارة اتصال المستخدمين
        public override async Task OnConnectedAsync()
        {
            var user = await _userManager.GetUserAsync(Context.User);
            if (user != null)
            {
                if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
                    // إعلام الأدمن بالاتصال الجديد
                    await Clients.Group("Admins").SendAsync("AdminConnected", user.UserName);
                }
                else
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, "Users");
                }
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var user = await _userManager.GetUserAsync(Context.User);
            if (user != null && await _userManager.IsInRoleAsync(user, "Admin"))
            {
                await Clients.Group("Admins").SendAsync("AdminDisconnected", user.UserName);
            }
            await base.OnDisconnectedAsync(exception);
        }
    }


    public class UserConversationDto
    {
        public string Id { get; set; }
        public string AdminName { get; set; }
        public string LastMessage { get; set; }
        public DateTime LastMessageAt { get; set; }
        public int UnreadCount { get; set; }
    }

}