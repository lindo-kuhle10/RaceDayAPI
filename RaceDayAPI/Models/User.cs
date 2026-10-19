using Microsoft.AspNetCore.Identity;

namespace RaceDayAPI.Models // Change this to match your project name!
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }
        public string Role { get; set; } // "Organiser" or "Participant"
    }
}
