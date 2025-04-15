namespace WebAPI.Models.DTOs
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public long TokenExpired { get; set; }
    }
}
