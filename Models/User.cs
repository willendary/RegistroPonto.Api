
using System.ComponentModel.DataAnnotations;

namespace RegistroPonto.Api.Models;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [Required]
    [StringLength(100)]
    public string Username { get; set; }

    [Required]
    public string PasswordHash { get; set; }

    [Required]
    public UserRole Role { get; set; }

    public virtual ICollection<TimeRecord> TimeRecords { get; set; } = new List<TimeRecord>();
}

public enum UserRole
{
    Employee,
    Admin
}
