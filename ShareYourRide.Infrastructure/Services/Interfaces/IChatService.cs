using ShareYourRide.Application.DTOs.Chat;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShareYourRide.Infrastructure.Services.Interfaces
{
    public interface IChatService
    {
        Task CreateThreadForApplicationAsync(Guid rideApplicationId, Guid driverUserId, Guid passengerUserId);
        Task<IReadOnlyList<ChatThreadDto>> GetMyThreadsAsync(Guid userId);
        Task<IReadOnlyList<ChatMessageDto>> GetMessagesAsync(Guid userId, Guid threadId);
        Task<ChatMessageDto> SendMessageAsync(Guid userId, Guid threadId, SendMessageDto dto);
    }
}