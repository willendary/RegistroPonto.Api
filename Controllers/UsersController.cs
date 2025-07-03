
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegistroPonto.Api.DTOs;
using RegistroPonto.Api.Models;
using RegistroPonto.Api.Services;

namespace RegistroPonto.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly UserManager _userManager;

    public UsersController(UserManager userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userManager.GetAllUsersAsync();
        var userDtos = users.Select(u => new UserDto
        {
            Id = u.Id,
            Name = u.Name,
            Username = u.Username,
            Role = u.Role.ToString()
        }).ToList();
        return Ok(userDtos);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var user = await _userManager.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Username = user.Username,
            Role = user.Role.ToString()
        });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] RegisterRequestDto request)
    {
        if (!Enum.TryParse(request.Role, true, out UserRole role))
        {
            return BadRequest("Invalid user role.");
        }

        var updatedUser = await _userManager.UpdateUserAsync(id, request.Name, request.Username, role);
        if (updatedUser == null)
        {
            return NotFound();
        }
        return Ok(new UserDto
        {
            Id = updatedUser.Id,
            Name = updatedUser.Name,
            Username = updatedUser.Username,
            Role = updatedUser.Role.ToString()
        });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var result = await _userManager.DeleteUserAsync(id);
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpGet("current")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return Unauthorized("User not found or not authenticated.");
        }

        var user = await _userManager.GetUserByIdAsync(userId);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        return Ok(new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Username = user.Username,
            Role = user.Role.ToString()
        });
    }
}
