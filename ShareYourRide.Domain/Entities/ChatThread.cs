using ShareYourRide.Domain.Common;
using System;
using System.Collections.Generic;

namespace ShareYourRide.Domain.Entities
{
    public class ChatThread : BaseEntity
    {
        public Guid RideApplicationId { get; set; }
        public RideApplication RideApplication { get; set; } = default!;

        public Guid DriverUserId { get; set; }
        public User DriverUser { get; set; } = default!;

        public Guid PassengerUserId { get; set; }
        public User PassengerUser { get; set; } = default!;

        public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }
}