using System.ComponentModel.DataAnnotations;

namespace BarberAppointmentSystemF.DTOs
{
    public class CreateAppointmentDto
    {
        [Required]
        public string CustomerName { get; set; } = string.Empty;
        
        [Required]
        public string Phone { get; set; } = string.Empty;
        
        public string? Email { get; set; }
        
        [Required]
        public DateTime AppointmentDate { get; set; }
        
        [Required]
        public string Service { get; set; } = string.Empty;
        
        public string? Notes { get; set; }
    }
    
    public class UpdateAppointmentDto
    {
        public string? CustomerName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public string? Service { get; set; }
        public string? Status { get; set; }
        public string? Notes { get; set; }
    }
    
    public class AppointmentResponseDto
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Email { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Service { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
    public class AvailableSlotDto
    {
        public DateTime DateTime { get; set; }
        public string TimeSlot { get; set; } = string.Empty; // "09:00", "10:00"
        public bool IsAvailable { get; set; }
    }

    public class CancelAppointmentDto
    {
        [Required(ErrorMessage = "Müşteri adı gerekli")]
        public string CustomerName { get; set; } = string.Empty;
    
        [Required(ErrorMessage = "Telefon numarası gerekli")]
        public string Phone { get; set; } = string.Empty;
    }

    public class MyAppointmentsDto
    {
        [Required(ErrorMessage = "Müşteri adı gerekli")]
        public string CustomerName { get; set; } = string.Empty;
    
        [Required(ErrorMessage = "Telefon numarası gerekli")]
        public string Phone { get; set; } = string.Empty;
    }
}