using System.Security.Claims;
using AuthHW.Data;
using AuthHW.DTOs.Chat;
using AuthHW.DTOs.Messages;
using AuthHW.DTOs.User;
using AuthHW.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthHW.Services;

public class ChatService
{
    private readonly AppDbContext _appDbContext;

    public ChatService(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    // 🔹 Получение всех чатов пользователя
    public async Task<List<ChatListItemDto>> GetMyChatsAsync(ClaimsPrincipal user)
    {
        var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        if (userId == 0)
            throw new UnauthorizedAccessException("UserId claim not found");

        return await _appDbContext.ChatParticipants
            .Where(p => p.UserId == userId)
            .Select(p => new ChatListItemDto
            {
                ChatId = p.Chat.Id,
                Type = p.Chat.Type.ToString(),
                Name = p.Chat.Type == ChatType.Private
                    ? p.Chat.Participants
                        .Where(x => x.UserId != userId)
                        .Select(x => x.User.Profile.DisplayName)
                        .FirstOrDefault() ?? "Unknown"
                    : p.Chat.Title,
                Avatar = p.Chat.Type == ChatType.Private
                    ? p.Chat.Participants
                        .Where(x => x.UserId != userId)
                        .Select(x => x.User.Profile.AvatarUrl)
                        .FirstOrDefault()
                    : p.Chat.AvatarUrl,
                Online = p.Chat.Type == ChatType.Private &&
                         p.Chat.Participants.Any(x =>
                             x.UserId != userId &&
                             x.User.Profile.LastSeenAt > DateTime.UtcNow.AddMinutes(-2)),
                LastMessage = p.Chat.Messages
                    .OrderByDescending(m => m.SentAt)
                    .Select(m => m.Content)
                    .FirstOrDefault(),
                LastMessageAt = p.Chat.Messages
                    .Select(m => m.SentAt)
                    .DefaultIfEmpty()
                    .Max(),
                UnreadCount = p.Chat.Messages
                    .Count(m => m.SentAt > p.LastReadAt),
                Partner = p.Chat.Type == ChatType.Private
                    ? p.Chat.Participants
                        .Where(x => x.UserId != userId)
                        .Select(x => new PartnerDto
                        {
                            Id = x.UserId,
                            DisplayName = x.User.Profile.DisplayName,
                            Avatar = x.User.Profile.AvatarUrl,
                            Online = x.User.Profile.LastSeenAt >
                                     DateTime.UtcNow.AddMinutes(-2),
                            LastSeenAt = x.User.Profile.LastSeenAt
                        })
                        .First()
                    : null
            })
            .OrderByDescending(c => c.LastMessageAt)
            .ToListAsync();
    }

    public async Task<List<Message>> GetChatMessagesAsync(int userId, int chatId, CancellationToken ct)
    {
        var isParticipant = await _appDbContext.ChatParticipants
            .AnyAsync(p => p.ChatId == chatId && p.UserId == userId, ct);

        if (!isParticipant)
            throw new UnauthorizedAccessException("User is not a participant of this chat");

        return await _appDbContext.Messages
            .Where(m => m.ChatId == chatId)
            .OrderBy(m => m.SentAt)
            .ToListAsync(ct);
    }

    // 🔹 Отправка одного сообщения
    public async Task<Message> SendMessageAsync(int userId, int chatId, string content, CancellationToken ct)
    {
        var isParticipant = await _appDbContext.ChatParticipants
            .AnyAsync(p => p.ChatId == chatId && p.UserId == userId, ct);

        if (!isParticipant)
            throw new UnauthorizedAccessException("User is not a participant of this chat");

        var message = new Message
        {
            ChatId = chatId,
            SenderId = userId,
            Content = content,
            SentAt = DateTime.UtcNow
        };

        _appDbContext.Messages.Add(message);
        await _appDbContext.SaveChangesAsync(ct);

        return message;
    }

    // 🔹 Создание приватного чата
    public async Task<Chat> OpenChatAsync(ClaimsPrincipal user, OpenChatDto dto)
    {
        var currentUserId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var existingChat = await _appDbContext.Chats
            .Where(c => c.Type == ChatType.Private)
            .Where(c => c.Participants.Any(p => p.UserId == currentUserId))
            .Where(c => c.Participants.Any(p => p.UserId == dto.UserId))
            .FirstOrDefaultAsync();

        if (existingChat != null)
            return existingChat;

        var chat = new Chat
        {
            Type = ChatType.Private,
            Participants = new List<ChatParticipant>
            {
                new ChatParticipant { UserId = currentUserId },
                new ChatParticipant { UserId = dto.UserId }
            }
        };

        _appDbContext.Chats.Add(chat);
        await _appDbContext.SaveChangesAsync();

        return chat;
    }
}
