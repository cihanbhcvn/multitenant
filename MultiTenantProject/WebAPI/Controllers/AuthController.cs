using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebAPI.Data;
using WebAPI.Models.DTOs;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        public AuthController(IConfiguration configuration,AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("login")]
        public ActionResult<LoginResponse> Login([FromBody] LoginReq loginReq)
        {
            var tenant = _context.Tenants.FirstOrDefault(x => x.TenantId == loginReq.TenantId);
            if (tenant == null)
                return Unauthorized("Invalid tenant.");

            if ((loginReq.Username == "admin" && loginReq.Password == "admin" && loginReq.TenantId == tenant.TenantId) || (loginReq.Username == "user" && loginReq.Password=="user" && loginReq.TenantId == tenant.TenantId))
            {
                var token = GenerateJwtToken(loginReq.Username,loginReq.TenantId);
                return Ok(new LoginResponse { Token = token });
            }

            return null;
        }

        private string GenerateJwtToken(string username,string tenantId)
        {
            var claims = new []
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, username == "admin" ? "Admin" : "User"),
                new Claim("TenantId",tenantId)
            };

            string secret = _configuration.GetValue<string>("Jwt:Secret");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "cihanbhcvn",
                audience: "cihanbhcvn",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
