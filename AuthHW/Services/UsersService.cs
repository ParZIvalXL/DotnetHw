using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AuthHW.Data;
using AuthHW.DTOs;
using AuthHW.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthHW.Services;

public class UsersService
{
    private readonly AppDbContext _context;

    public UsersService(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<UserAccount> GetUserById(int id)
    {
        var user = await _context.UserAccounts
            .FirstOrDefaultAsync(u => u.Id == id);
        if(user == null)
            throw new ArgumentException("Пользователь не найден");
        
        return user;
    }
    
    public async Task<UserAccount> GetUserByTag(string tag)
    {
        var user = await _context.UserAccounts
            .FirstOrDefaultAsync(u => u.Tag == tag);
        if(user == null)
            throw new ArgumentException("Пользователь не найден");
        
        return user;
    }
    
    public async Task<UserAccount> GetUserMe(ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
            throw new UnauthorizedAccessException("UserId claim not found");

        var userId = int.Parse(userIdClaim.Value);

        var account = await _context.UserAccounts
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (account == null)
            throw new ArgumentException("Пользователь не найден");

        return account;
    }

    
    public async Task<List<UserSearchDto>> SearchUsers(string tag)
    {
        var users = await _context.UserAccounts
            .Where(u => u.Tag.ToLower().Contains(tag.ToLower()))
            .Select(u => new UserSearchDto(
                u.Id,
                u.Username,
                u.Tag,
                null
            ))
            .Take(10)
            .ToListAsync();

        return users;
    }

}