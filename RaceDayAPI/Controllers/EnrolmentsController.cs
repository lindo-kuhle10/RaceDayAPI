using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.Models;

namespace RaceDayAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnrolmentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EnrolmentsController(AppDbContext context)
        {
            _context = context;
        }

        private bool IsLoggedIn() => HttpContext.Session.GetString("UserId") != null;
        private bool IsParticipant() => HttpContext.Session.GetString("UserRole") == "Participant";
        private bool IsOrganiser() => HttpContext.Session.GetString("UserRole") == "Organiser";

        // POST: api/Enrolments (Participant only)
        [HttpPost]
        public async Task<IActionResult> Enrol([FromBody] EnrolmentRequest request)
        {
            if (!IsLoggedIn()) return Unauthorized("You must log in first.");
            if (!IsParticipant()) return StatusCode(403, "Only Participants can enrol in events.");

            string userId = HttpContext.Session.GetString("UserId");

            // Check if the category exists and belongs to the event
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == request.CategoryId && c.EventId == request.EventId);
            if (category == null) return BadRequest("Invalid Category or Event ID.");

            // Check if they are already enrolled in this event
            var alreadyEnrolled = await _context.Enrolments.AnyAsync(e => e.UserId == userId && e.EventId == request.EventId);
            if (alreadyEnrolled) return BadRequest("You are already enrolled in this event.");

            var enrolment = new Enrolment
            {
                UserId = userId,
                EventId = request.EventId,
                CategoryId = request.CategoryId
            };

            _context.Enrolments.Add(enrolment);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Successfully enrolled!", enrolmentId = enrolment.Id });
        }

        // GET: api/Enrolments/event/5 (Organiser only - to view who enrolled)
        [HttpGet("event/{eventId}")]
        public async Task<IActionResult> GetEnrolmentsForEvent(int eventId)
        {
            if (!IsLoggedIn()) return Unauthorized("You must log in first.");
            if (!IsOrganiser()) return StatusCode(403, "Only Organisers can view all enrolments.");

            var enrolments = await _context.Enrolments
                .Include(e => e.User)
                .Include(e => e.Category)
                .Where(e => e.EventId == eventId)
                .ToListAsync();

            return Ok(enrolments);
        }

        // PUT: api/Enrolments/result/5 (Organiser only)
        [HttpPut("result/{enrolmentId}")]
        public async Task<IActionResult> CaptureResult(int enrolmentId, [FromBody] ResultRequest request)
        {
            if (!IsLoggedIn()) return Unauthorized("You must log in first.");
            if (!IsOrganiser()) return StatusCode(403, "Only Organisers can capture results.");

            var enrolment = await _context.Enrolments.FindAsync(enrolmentId);
            if (enrolment == null) return NotFound("Enrolment not found.");

            enrolment.FinishTime = request.FinishTime;
            enrolment.Position = request.Position;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Result captured successfully." });
        }

        // GET: api/Enrolments/my-results (Participant only)
        [HttpGet("my-results")]
        public async Task<IActionResult> GetMyResults()
        {
            if (!IsLoggedIn()) return Unauthorized("You must log in first.");
            if (!IsParticipant()) return StatusCode(403, "Only Participants can view their results.");

            string userId = HttpContext.Session.GetString("UserId");

            var results = await _context.Enrolments
                .Include(e => e.Event)
                .Include(e => e.Category)
                .Where(e => e.UserId == userId)
                .Select(e => new
                {
                    EventName = e.Event.Name,
                    CategoryName = e.Category.Name,
                    e.FinishTime,
                    e.Position
                })
                .ToListAsync();

            return Ok(results);
        }
    }

    // --- DATA TRANSFER OBJECTS (DTOs) ---
    // These must be OUTSIDE the controller class, but INSIDE the namespace.

    public class EnrolmentRequest
    {
        public int EventId { get; set; }
        public int CategoryId { get; set; }
    }

    public class ResultRequest
    {
        public string FinishTime { get; set; } // e.g., "02:15:30"
        public int Position { get; set; }
    }
}