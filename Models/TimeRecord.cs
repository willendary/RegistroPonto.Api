
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegistroPonto.Api.Models;

public class TimeRecord
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [ForeignKey("UserId")]
    public virtual User User { get; set; }

    [Required]
    public DateTime Time { get; set; }

    [Required]
    public RecordType Type { get; set; }
}

public enum RecordType
{
    ClockIn,
    ClockOut,
    StartBreak,
    EndBreak
}
