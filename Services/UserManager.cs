
using Microsoft.EntityFrameworkCore;
using RegistroPonto.Api.Data;
using RegistroPonto.Api.Models;

namespace RegistroPonto.Api.Services;

public class UserManager
{
    private readonly ApplicationDbContext _context;
    private readonly PasswordService _passwordService;

    public UserManager(ApplicationDbContext context, PasswordService passwordService)
    {
        _context = context;
        _passwordService = passwordService;
    }

    public async Task<User> RegisterUserAsync(string name, string username, string password, UserRole role)
    {
        if (await _context.Users.AnyAsync(u => u.Username == username))
        {
            throw new InvalidOperationException("Username already exists.");
        }

        var user = new User
        {
            Name = name,
            Username = username,
            PasswordHash = _passwordService.HashPassword(password),
            Role = role
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User> AuthenticateUserAsync(string username, string password)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
        if (user == null || !_passwordService.VerifyPassword(password, user.PasswordHash))
        {
            return null;
        }
        return user;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User> GetUserByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User> UpdateUserAsync(int id, string name, string username, UserRole role)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return null;
        }

        user.Name = name;
        user.Username = username;
        user.Role = role;

        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return false;
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }
}
