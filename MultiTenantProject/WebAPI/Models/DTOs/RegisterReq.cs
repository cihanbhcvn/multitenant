namespace WebAPI.Models.DTOs;

public class RegisterReq
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "User";
    public string TenantId { get; set; }
}