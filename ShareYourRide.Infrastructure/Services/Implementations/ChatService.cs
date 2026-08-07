using ShareYourRide.Application.DTOs.Chat;
using ShareYourRide.Domain.Entities;
using ShareYourRide.Infrastructure.Repositories.Interfaces;
using ShareYourRide.Infrastructure.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShareYourRide.Infrastructure.Services.Implementations
{
    public class ChatService : IChatService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChatService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CreateThreadForApplicationAsync(Guid rideApplicationId, Guid driverUserId, Guid passengerUserId)
        {
            var existing = await _unitOfWork.ChatThreads.SingleOrDefaultAsync(c => c.RideApplicationId == rideApplicationId);
            if (existing != null) return;

            await _unitOfWork.ChatThreads.AddAsync(new ChatThread
            {
                RideApplicationId = rideApplicationId,
                DriverUserId = driverUserId,
                PassengerUserId = passengerUserId
            });

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<ChatThreadDto>> GetMyThreadsAsync(Guid userId)
        {
            var threads = await _unitOfWork.ChatThreads.FindAsync(c =>
                c.DriverUserId == userId || c.PassengerUserId == userId);

            var result = new List<ChatThreadDto>();
            foreach (var t in threads)
            {
                var otherPartyId = t.DriverUserId == userId ? t.PassengerUserId : t.DriverUserId;
                var otherParty = await _unitOfWork.Users.GetByIdAsync(otherPartyId);

                var messages = await _unitOfWork.ChatMessages.FindAsync(m => m.ChatThreadId == t.Id);
                var last = messages.OrderByDescending(m => m.CreatedAt).FirstOrDefault();
                var unread = messages.Count(m => m.SenderUserId != userId && !m.IsRead);

                result.Add(new ChatThreadDto
                {
                    Id = t.Id,
                    RideApplicationId = t.RideApplicationId,
                    OtherPartyFullName = otherParty != null ? $"{otherParty.FirstName} {otherParty.LastName}" : "N/A",
                    LastMessage = last?.Content,
                    LastMessageAt = last?.CreatedAt,
                    UnreadCount = unread
                });
            }

            return result.OrderByDescending(t => t.LastMessageAt).ToList();
        }

        public async Task<IReadOnlyList<ChatMessageDto>> GetMessagesAsync(Guid userId, Guid threadId)
        {
            var thread = await GetOwnedThreadAsync(userId, threadId);

            var messages = await _unitOfWork.ChatMessages.FindAsync(m => m.ChatThreadId == thread.Id);
            var ordered = messages.OrderBy(m => m.CreatedAt).ToList();

            var unreadFromOther = ordered.Where(m => m.SenderUserId != userId && !m.IsRead).ToList();
            foreach (var m in unreadFromOther)
            {
                m.IsRead = true;
                _unitOfWork.ChatMessages.Update(m);
            }
            if (unreadFromOther.Any())
                await _unitOfWork.SaveChangesAsync();

            var result = new List<ChatMessageDto>();
            foreach (var m in ordered)
            {
                var sender = await _unitOfWork.Users.GetByIdAsync(m.SenderUserId);
                result.Add(new ChatMessageDto
                {
                    Id = m.Id,
                    SenderUserId = m.SenderUserId,
                    SenderFullName = sender != null ? $"{sender.FirstName} {sender.LastName}" : "N/A",
                    Content = m.Content,
                    IsMine = m.SenderUserId == userId,
                    CreatedAt = m.CreatedAt
                });
            }

            return result;
        }

        public async Task<ChatMessageDto> SendMessageAsync(Guid userId, Guid threadId, SendMessageDto dto)
        {
            var thread = await GetOwnedThreadAsync(userId, threadId);

            if (string.IsNullOrWhiteSpace(dto.Content))
                throw new InvalidOperationException("Mesaj boş ola bilməz.");

            var message = new ChatMessage
            {
                ChatThreadId = thread.Id,
                SenderUserId = userId,
                Content = dto.Content.Trim()
            };

            await _unitOfWork.ChatMessages.AddAsync(message);
            await _unitOfWork.SaveChangesAsync();

            var sender = await _unitOfWork.Users.GetByIdAsync(userId);

            return new ChatMessageDto
            {
                Id = message.Id,
                SenderUserId = userId,
                SenderFullName = sender != null ? $"{sender.FirstName} {sender.LastName}" : "N/A",
                Content = message.Content,
                IsMine = true,
                CreatedAt = message.CreatedAt
            };
        }

        private async Task<ChatThread> GetOwnedThreadAsync(Guid userId, Guid threadId)
        {
            var thread = await _unitOfWork.ChatThreads.GetByIdAsync(threadId)
                ?? throw new InvalidOperationException("Söhbət tapılmadı.");

            if (thread.DriverUserId != userId && thread.PassengerUserId != userId)
                throw new InvalidOperationException("Bu söhbətə icazəniz yoxdur.");

            return thread;
        }
    }
}