using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.Models;

namespace RaceDayAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        private bool IsLoggedIn() => HttpContext.Session.GetString("UserId") != null;
        private bool IsOrganiser() => HttpContext.Session.GetString("UserRole") == "Organiser";

        // GET: api/Categories/event/5 (Both roles can view categories for an event)
        [HttpGet("event/{eventId}")]
        public async Task<IActionResult> GetCategoriesForEvent(int eventId)
        {
            if (!IsLoggedIn()) return Unauthorized("You must log in first.");

            var categories = await _context.Categories.Where(c => c.EventId == eventId).ToListAsync();
            return Ok(categories);
        }

        // POST: api/Categories (Organiser only)
        [HttpPost]
        public async Task<IActionResult> CreateCategory(Category newCategory)
        {
            if (!IsLoggedIn()) return Unauthorized("You must log in first.");
            if (!IsOrganiser()) return StatusCode(403, "Only Organisers can create categories.");

            // Check if the event exists
            var eventExists = await _context.Events.AnyAsync(e => e.Id == newCategory.EventId);
            if (!eventExists) return BadRequest("The specified Event does not exist.");

            _context.Categories.Add(newCategory);
            await _context.SaveChangesAsync();
            return Ok(newCategory);
        }
    }
}