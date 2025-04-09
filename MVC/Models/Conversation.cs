using mvc.Models.Authorize;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mvc.Models
{
    public class Conversation
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string UserId { get; set; } // المستخدم العادي (مطلوب)

        public string? AdminId { get; set; } // الأدمن الذي يرد (يظل null في الرسائل العامة)

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastMessageAt { get; set; }
        public bool IsClosed { get; set; } = false;

        // هل هذه المحادثة رسالة عامة لكل الأدمنز؟
        public bool IsAdminBroadcast { get; set; } = false;

        // تتبع حالة الرسالة
        public bool IsReadByAdmin { get; set; } = false; // إضافة جديدة
        public bool IsReadByUser { get; set; } = false; // إضافة جديدة

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        [ForeignKey("AdminId")]
        public virtual ApplicationUser? Admin { get; set; }

        public virtual ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }
}