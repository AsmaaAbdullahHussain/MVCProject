using System;

namespace mvc.ViewModels
{
    public class ChatMessageViewModel
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string SenderId { get; set; }
        public string SenderName { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
        // Adding ConversationId property that might be needed
        public string ConversationId { get; set; }
    }
}
