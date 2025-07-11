using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using BarberAppointmentSystemF.Data;
using BarberAppointmentSystemF.Models;
using BarberAppointmentSystemF.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace BarberAppointmentSystemF.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public AppointmentController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/appointments
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<AppointmentResponseDto>>> GetAppointments()
        {
            var appointments = await _context.Appointments
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();

            var response = _mapper.Map<List<AppointmentResponseDto>>(appointments);
            return Ok(response);
        }

        // GET: api/appointments/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<AppointmentResponseDto>> GetAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);

            if (appointment == null)
                return NotFound();

            var response = _mapper.Map<AppointmentResponseDto>(appointment);
            return Ok(response);
        }

        // POST: api/appointments
        [HttpPost]
        public async Task<ActionResult<AppointmentResponseDto>> CreateAppointment(CreateAppointmentDto createDto)
        {
            // Aynı tarih/saatte randevu var mı kontrol et
            var existingAppointment = await _context.Appointments
                .AnyAsync(a => a.AppointmentDate == createDto.AppointmentDate);

            if (existingAppointment)
                return BadRequest(new { message = "Bu saat için randevu mevcut!" });

            var appointment = _mapper.Map<Appointment>(createDto);
            
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            var response = _mapper.Map<AppointmentResponseDto>(appointment);
            return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, response);
        }

        // PUT: api/appointments/5
        [HttpPut("{id}")]
        public async Task<ActionResult<AppointmentResponseDto>> UpdateAppointment(int id, UpdateAppointmentDto updateDto)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
                return NotFound();

            _mapper.Map(updateDto, appointment);
            appointment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var response = _mapper.Map<AppointmentResponseDto>(appointment);
            return Ok(response);
        }

        // DELETE: api/appointments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
                return NotFound();

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/appointments/today
        [HttpGet("today")]
        public async Task<ActionResult<IEnumerable<AppointmentResponseDto>>> GetTodayAppointments()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var appointments = await _context.Appointments
                .Where(a => a.AppointmentDate >= today && a.AppointmentDate < tomorrow)
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();

            var response = _mapper.Map<List<AppointmentResponseDto>>(appointments);
            return Ok(response);
        }
        // 🌐 PUBLIC - Müşteri için uygun saatleri göster
[HttpGet("available-slots")]
public async Task<ActionResult<IEnumerable<AvailableSlotDto>>> GetAvailableSlots(
    [FromQuery] DateTime date)
{
    // Çalışma saatleri: 09:00-18:00, her saat başı
    var workingHours = new List<DateTime>();
    var startTime = date.Date.AddHours(9);  // 09:00
    var endTime = date.Date.AddHours(18);   // 18:00

    for (var time = startTime; time < endTime; time = time.AddHours(1))
    {
        workingHours.Add(time);
    }

    // Dolu saatleri çıkar
    var bookedSlots = await _context.Appointments
        .Where(a => a.AppointmentDate.Date == date.Date && 
                   a.Status != "Cancelled")
        .Select(a => a.AppointmentDate)
        .ToListAsync();

    var availableSlots = workingHours
        .Where(slot => !bookedSlots.Contains(slot) && slot > DateTime.Now)
        .Select(slot => new AvailableSlotDto
        {
            DateTime = slot,
            TimeSlot = slot.ToString("HH:mm"),
            IsAvailable = true
        })
        .ToList();

    return Ok(availableSlots);
}

// 🌐 PUBLIC - Müşteri randevu iptal etme
[HttpPost("cancel")]
public async Task<IActionResult> CancelAppointment(CancelAppointmentDto cancelDto)
{
    var appointment = await _context.Appointments
        .FirstOrDefaultAsync(a => 
            a.Phone == cancelDto.Phone && 
            a.CustomerName.ToLower() == cancelDto.CustomerName.ToLower() &&
            a.Status != "Cancelled");

    if (appointment == null)
        return NotFound(new { message = "Randevu bulunamadı" });

    // Randevu tarihine 2 saatten az kalmışsa iptal edilemez
    if (appointment.AppointmentDate <= DateTime.Now.AddHours(2))
        return BadRequest(new { message = "Randevu tarihine 2 saatten az kala iptal edilemez" });

    appointment.Status = "Cancelled";
    appointment.UpdatedAt = DateTime.UtcNow;
    appointment.Notes += $" - {DateTime.Now:dd.MM.yyyy HH:mm}'de müşteri tarafından iptal edildi";

    await _context.SaveChangesAsync();

    return Ok(new { message = "Randevunuz başarıyla iptal edildi" });
}

// 🌐 PUBLIC - Müşteri kendi randevularını görme
        [HttpPost("my-appointments")]
        public async Task<ActionResult<IEnumerable<AppointmentResponseDto>>> GetMyAppointments(
         MyAppointmentsDto searchDto)
        { 
            var appointments = await _context.Appointments
        .Where(a => a.Phone == searchDto.Phone && 
                   a.CustomerName.ToLower() == searchDto.CustomerName.ToLower())
        .OrderByDescending(a => a.AppointmentDate)
        .ToListAsync();

            var response = _mapper.Map<List<AppointmentResponseDto>>(appointments); 
            return Ok(response);
}
    }
    
}