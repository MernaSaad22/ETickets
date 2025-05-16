using System.ComponentModel.DataAnnotations;
namespace ETickets.ViewModels
{
    public class RegisterWithRoleVM
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Passwords don't match.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Address")]
        public string? Address { get; set; }

        [Required]
        [Range(1, 120, ErrorMessage = "Age must be 1-120")]
        public int Age { get; set; }

        [Display(Name = "Gender")]
        public string? Gender { get; set; }

        [Required]
        [Display(Name = "Role")]
        public string RoleName { get; set; }
    }
}
