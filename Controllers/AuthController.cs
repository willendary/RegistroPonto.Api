
using Microsoft.AspNetCore.Mvc;
using RegistroPonto.Api.DTOs;
using RegistroPonto.Api.Models;
using RegistroPonto.Api.Services;

namespace RegistroPonto.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager _userManager;
    private readonly TokenService _tokenService;

    public AuthController(UserManager userManager, TokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        if (!Enum.TryParse(request.Role, true, out UserRole role))
        {
            return BadRequest("Invalid user role.");
        }

        try
        {
            var user = await _userManager.RegisterUserAsync(request.Name, request.Username, request.Password, role);
            return Ok(new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Username = user.Username,
                Role = user.Role.ToString()
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var user = await _userManager.AuthenticateUserAsync(request.Username, request.Password);
        if (user == null)
        {
            return Unauthorized("Invalid credentials.");
        }

        var token = _tokenService.GenerateToken(user);
        return Ok(new { Token = token });
    }
}
