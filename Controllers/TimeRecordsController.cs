
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegistroPonto.Api.DTOs;
using RegistroPonto.Api.Models;
using RegistroPonto.Api.Services;
using System.Security.Claims;

namespace RegistroPonto.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TimeRecordsController : ControllerBase
{
    private readonly TimeRecordService _timeRecordService;

    public TimeRecordsController(TimeRecordService timeRecordService)
    {
        _timeRecordService = timeRecordService;
    }

    private int GetUserId()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterTime([FromQuery] RecordType type)
    {
        var userId = GetUserId();
        try
        {
            var record = await _timeRecordService.RegisterTimeAsync(userId, type);
            return Ok(new TimeRecordDto
            {
                Id = record.Id,
                UserId = record.UserId,
                Time = record.Time,
                Type = record.Type.ToString()
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetUserTimeRecords(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var userId = GetUserId();
        var records = await _timeRecordService.GetUserTimeRecordsAsync(userId, startDate, endDate);
        var recordDtos = records.Select(r => new TimeRecordDto
        {
            Id = r.Id,
            UserId = r.UserId,
            Time = r.Time,
            Type = r.Type.ToString()
        }).ToList();
        return Ok(recordDtos);
    }

    [HttpGet("worked-hours")]
    public async Task<IActionResult> GetWorkedHours(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var userId = GetUserId();
        var workedHours = await _timeRecordService.CalculateWorkedHoursAsync(userId, startDate, endDate);
        return Ok(new { UserId = userId, StartDate = startDate, EndDate = endDate, WorkedHours = workedHours.ToString() });
    }

    [HttpGet("overtime")]
    public async Task<IActionResult> GetOvertime(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] int standardWorkDayHours = 8)
    {
        var userId = GetUserId();
        var overtime = await _timeRecordService.CalculateOvertimeAsync(userId, startDate, endDate, TimeSpan.FromHours(standardWorkDayHours));
        return Ok(new { UserId = userId, StartDate = startDate, EndDate = endDate, Overtime = overtime.ToString() });
    }
}
