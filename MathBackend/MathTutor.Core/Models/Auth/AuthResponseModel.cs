namespace MathTutor.Core.Models.Auth;

public class AuthResponseModel
{
    public bool Success { get; set; }
    public string Token { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public UserModel User { get; set; } = null!;
    public List<string> Errors { get; set; } = new List<string>();
} 