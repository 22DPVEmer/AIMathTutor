using System.ComponentModel.DataAnnotations;

namespace MathTutor.Core.Models;

public class ChangePasswordModel
{
    [Required]
    public string UserId { get; set; } = string.Empty;
    
    [Required]
    public string CurrentPassword { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string NewPassword { get; set; } = string.Empty;
} 