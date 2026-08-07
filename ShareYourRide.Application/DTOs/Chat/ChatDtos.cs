using System;
using System.ComponentModel.DataAnnotations;

namespace ShareYourRide.Application.DTOs.Chat
{
    public class ChatThreadDto
    {
        public Guid Id { get; set; }
        public Guid RideApplicationId { get; set; }
        public string OtherPartyFullName { get; set; } = default!;
        public string? LastMessage { get; set; }
        public DateTime? LastMessageAt { get; set; }
        public int UnreadCount { get; set; }
    }

    public class ChatMessageDto
    {
        public Guid Id { get; set; }
        public Guid SenderUserId { get; set; }
        public string SenderFullName { get; set; } = default!;
        public string Content { get; set; } = default!;
        public bool IsMine { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class SendMessageDto
    {
        [Required] public string Content { get; set; } = default!;
    }
}