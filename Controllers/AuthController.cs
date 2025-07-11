using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BarberAppointmentSystemF.Data;
using BarberAppointmentSystemF.Models;
using BarberAppointmentSystemF.DTOs;

namespace BarberAppointmentSystemF.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == loginDto.Username && u.IsActive);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                return Unauthorized(new { message = "Kullanıcı adı veya şifre hatalı" });

            // Basit token (JWT olmadan MVP için)
            var token = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{user.Username}:{DateTime.Now}"));

            return Ok(new AuthResponseDto
            {
                Token = token,
                Username = user.Username,
                Role = user.Role,
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            });
        }

        [HttpPost("create-admin")]
        public async Task<IActionResult> CreateAdmin()
        {
            // Admin kullanıcısı var mı kontrol et
            var existingAdmin = await _context.Users.AnyAsync(u => u.Username == "admin");
            if (existingAdmin)
                return BadRequest(new { message = "Admin kullanıcısı zaten var" });

            var admin = new User
            {
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                Email = "admin@barbershop.com",
                Role = "Admin",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(admin);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Admin kullanıcısı oluşturuldu", username = "admin", password = "admin123" });
        }
    }
}