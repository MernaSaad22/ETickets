using Microsoft.AspNetCore.Identity;

namespace ETickets.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string? Address { get; set; }
        public int Age { get; set; }
        public string? Gender { get; set; }
        public string? ProfileImage { get; set; }

    }
}
