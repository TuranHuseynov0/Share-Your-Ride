using ShareYourRide.Domain.Common;
using System;

namespace ShareYourRide.Domain.Entities
{
    public class ChatMessage : BaseEntity
    {
        public Guid ChatThreadId { get; set; }
        public ChatThread ChatThread { get; set; } = default!;

        public Guid SenderUserId { get; set; }
        public User SenderUser { get; set; } = default!;

        public string Content { get; set; } = default!;
        public bool IsRead { get; set; } = false;
    }
}