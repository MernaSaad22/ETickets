using System.ComponentModel.DataAnnotations;

namespace ETickets.ViewModels
{
    public class ResendEmailVM
    {
        [Required]

        public string EmailOrUserName { get; set; } = null!;
    }
}
