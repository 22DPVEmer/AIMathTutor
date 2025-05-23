namespace MathTutor.Core.Models;

public class UserModel
{
    public string Id { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLogin { get; set; }
    public bool IsVerified { get; set; }
    public List<string> Roles { get; set; } = new List<string>();
} 