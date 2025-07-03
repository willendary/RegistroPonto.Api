
using RegistroPonto.Api.Models;

namespace RegistroPonto.Api.DTOs;

public class TimeRecordDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime Time { get; set; }
    public string Type { get; set; }
}
