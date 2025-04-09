using System.ComponentModel.DataAnnotations;

namespace MathTutor.Core.Models;

public class UserRoleModel
{
    [Required]
    public string UserId { get; set; } = string.Empty;
    
    [Required]
    public string RoleName { get; set; } = string.Empty;
} 