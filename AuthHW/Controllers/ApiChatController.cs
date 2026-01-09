using System.Security.Claims;
using AuthHW.DTOs.Chat;
using AuthHW.DTOs.Messages;
using AuthHW.Entities;
using AuthHW.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthHW.Controllers;

[Route("api/chats")]
[ApiController]
[Authorize]
public class ApiChatController : ControllerBase
{
    private readonly ChatService _chatService;

    public ApiChatController(ChatService chatService)
    {
        _chatService = chatService;
    }

    // 🔹 Получить чаты текущего пользователя
    [HttpGet("my-chats")]
    public async Task<IActionResult> GetMyChats()
    {
        var chats = await _chatService.GetMyChatsAsync(User);
        return Ok(chats);
    }

    // 🔹 Создать или открыть чат
    [HttpPost]
    public async Task<IActionResult> OpenChat([FromBody] OpenChatDto dto)
    {
        var chat = await _chatService.OpenChatAsync(User, dto);
        return Ok(chat);
    }

    // 🔹 Получить сообщения чата
    [HttpGet("{chatId}/messages")]
    public async Task<IActionResult> GetMessages(int chatId, CancellationToken ct)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var messages = await _chatService.GetChatMessagesAsync(userId, chatId, ct);

        var result = messages.Select(m => new
        {
            m.Id,
            m.ChatId,
            m.SenderId,
            m.Content,
            Time = m.SentAt,
            Type = "text", // фиксируем тип для фронта
            Read = true
        });

        return Ok(result);
    }

    // 🔹 Отправить сообщение
    [HttpPost("{chatId}/messages")]
    public async Task<IActionResult> SendMessage(int chatId, [FromBody] SendMessageDto dto, CancellationToken ct)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var message = await _chatService.SendMessageAsync(userId, chatId, dto.Content, ct);

        return Ok(new
        {
            message.Id,
            message.ChatId,
            message.SenderId,
            message.Content,
            Time = message.SentAt,
            Type = "text",
            Read = true
        });
    }
}
