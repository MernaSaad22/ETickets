using System.ComponentModel.DataAnnotations;

namespace ETickets.ViewModels
{
    public class ForgetPasswordVM
    {
        [Required]
        public string EmailOrUserName { get; set; } = null!;
    }
}
