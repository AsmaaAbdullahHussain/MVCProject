using mvc.Models.Authorize;
using mvc.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class ChatMessage
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string ConversationId { get; set; }

    [Required]
    public string SenderId { get; set; }

    [Required]
    [MaxLength(1000)] // تحديد طول الرسالة
    public string Content { get; set; }

    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; } = false;

    // إضافة نوع الرسالة (نص/صورة/ملف)
    public MessageType Type { get; set; } = MessageType.Text;

    // إضافة حالة التسليم (اختياري)
    public bool IsDelivered { get; set; } = false;

    // Navigation properties
    [ForeignKey("ConversationId")]
    public virtual Conversation Conversation { get; set; }

    [ForeignKey("SenderId")]
    public virtual ApplicationUser Sender { get; set; }
}

// إضافة enum لأنواع الرسائل
public enum MessageType
{
    Text,       // رسالة نصية عادية
    Image,      // صورة
    File,       // ملف مرفق
    System      // رسالة نظام (مثل "تم إغلاق المحادثة")
}