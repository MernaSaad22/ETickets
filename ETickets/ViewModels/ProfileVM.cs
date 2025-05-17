namespace ETickets.ViewModels
{
    public class ProfileVM
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string? Address { get; set; }
        public int Age { get; set; }
        public string? Gender { get; set; }
        public string? ProfileImage { get; set; }
        public IFormFile? NewProfileImage { get; set; } 
    }
}
