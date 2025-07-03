
using Microsoft.EntityFrameworkCore;
using RegistroPonto.Api.Data;
using RegistroPonto.Api.Models;

namespace RegistroPonto.Api.Services;

public class TimeRecordService
{
    private readonly ApplicationDbContext _context;

    public TimeRecordService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TimeRecord> RegisterTimeAsync(int userId, RecordType type)
    {
        // Basic validation for duplicate records on the same day
        if (type == RecordType.ClockIn || type == RecordType.ClockOut)
        {
            var todayRecords = await _context.TimeRecords
                .Where(tr => tr.UserId == userId && tr.Time.Date == DateTime.Today)
                .ToListAsync();

            if (type == RecordType.ClockIn && todayRecords.Any(tr => tr.Type == RecordType.ClockIn))
            {
                throw new InvalidOperationException("Clock-in already registered for today.");
            }
            if (type == RecordType.ClockOut && todayRecords.Any(tr => tr.Type == RecordType.ClockOut))
            {
                throw new InvalidOperationException("Clock-out already registered for today.");
            }
        }

        var timeRecord = new TimeRecord
        {
            UserId = userId,
            Time = DateTime.Now,
            Type = type
        };

        _context.TimeRecords.Add(timeRecord);
        await _context.SaveChangesAsync();
        return timeRecord;
    }

    public async Task<List<TimeRecord>> GetUserTimeRecordsAsync(int userId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.TimeRecords.Where(tr => tr.UserId == userId);

        if (startDate.HasValue)
        {
            query = query.Where(tr => tr.Time.Date >= startDate.Value.Date);
        }

        if (endDate.HasValue)
        {
            query = query.Where(tr => tr.Time.Date <= endDate.Value.Date);
        }

        return await query.OrderBy(tr => tr.Time).ToListAsync();
    }

    public async Task<TimeSpan> CalculateWorkedHoursAsync(int userId, DateTime startDate, DateTime endDate)
    {
        var records = await GetUserTimeRecordsAsync(userId, startDate, endDate);

        TimeSpan totalWorkedTime = TimeSpan.Zero;
        DateTime? clockIn = null;
        DateTime? breakStart = null;

        foreach (var record in records.OrderBy(r => r.Time))
        {
            switch (record.Type)
            {
                case RecordType.ClockIn:
                    clockIn = record.Time;
                    break;
                case RecordType.ClockOut:
                    if (clockIn.HasValue)
                    {
                        totalWorkedTime += (record.Time - clockIn.Value);
                        clockIn = null;
                    }
                    break;
                case RecordType.StartBreak:
                    breakStart = record.Time;
                    break;
                case RecordType.EndBreak:
                    if (breakStart.HasValue)
                    {
                        totalWorkedTime -= (record.Time - breakStart.Value);
                        breakStart = null;
                    }
                    break;
            }
        }
        return totalWorkedTime;
    }

    public async Task<TimeSpan> CalculateOvertimeAsync(int userId, DateTime startDate, DateTime endDate, TimeSpan standardWorkDay)
    {
        var workedHours = await CalculateWorkedHoursAsync(userId, startDate, endDate);
        var totalStandardTime = (endDate - startDate).TotalDays * standardWorkDay.TotalHours;

        if (workedHours.TotalHours > totalStandardTime)
        {
            return TimeSpan.FromHours(workedHours.TotalHours - totalStandardTime);
        }
        return TimeSpan.Zero;
    }
}
