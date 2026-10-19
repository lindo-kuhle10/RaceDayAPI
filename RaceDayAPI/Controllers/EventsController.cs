using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.Models;

namespace RaceDayAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EventsController(AppDbContext context)
        {
            _context = context;
        }

        // Helper method to check if the user is logged in
        private bool IsLoggedIn() => HttpContext.Session.GetString("UserId") != null;

        // Helper method to check if the logged-in user is an Organiser
        private bool IsOrganiser() => HttpContext.Session.GetString("UserRole") == "Organiser";

        // GET: api/Events (Both roles can view)
        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            if (!IsLoggedIn()) return Unauthorized("You must log in first.");

            var events = await _context.Events.Include(e => e.Categories).ToListAsync();
            return Ok(events);
        }

        // POST: api/Events (Organiser only)
        [HttpPost]
        public async Task<IActionResult> CreateEvent(Event newEvent)
        {
            if (!IsLoggedIn()) return Unauthorized("You must log in first.");
            if (!IsOrganiser()) return StatusCode(403, "Only Organisers can create events.");

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();
            return Ok(newEvent);
        }

        // DELETE: api/Events/5 (Organiser only)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            if (!IsLoggedIn()) return Unauthorized("You must log in first.");
            if (!IsOrganiser()) return StatusCode(403, "Only Organisers can delete events.");

            var ev = await _context.Events.FindAsync(id);
            if (ev == null) return NotFound("Event not found.");

            _context.Events.Remove(ev);
            await _context.SaveChangesAsync();
            return Ok("Event deleted successfully.");
        }
    }
}