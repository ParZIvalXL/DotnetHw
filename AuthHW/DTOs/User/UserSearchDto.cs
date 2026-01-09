namespace AuthHW.DTOs;

public record UserSearchDto(
    int Id,
    string Username,
    string Tag,
    string? AvatarUrl
);