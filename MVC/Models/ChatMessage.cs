using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mvc.Models
{
    public class ChatMessage
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Content { get; set; }
        
        [Required]
        public string UserId { get; set; }
        
        // Adding SenderId property that was missing
        public string SenderId { get; set; }
        
        public string SenderName { get; set; }
        
        public string RecipientId { get; set; }
        
        // Adding ConversationId property that was missing
        public string ConversationId { get; set; }
        
        public bool IsFromAdmin { get; set; }
        
        public bool IsRead { get; set; }
        
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}