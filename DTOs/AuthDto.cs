using System.ComponentModel.DataAnnotations;

namespace BarberAppointmentSystemF.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Kullanıcı adı gerekli")]
        [StringLength(50, ErrorMessage = "Kullanıcı adı en fazla 50 karakter olabilir")]
        public string Username { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Şifre gerekli")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Şifre en az 3 karakter olmalı")]
        public string Password { get; set; } = string.Empty;
    }
    
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }
}