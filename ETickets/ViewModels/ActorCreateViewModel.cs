using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ETickets.ViewModels
{
    public class ActorCreateViewModel
    {
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string Bio { get; set; }

        public IFormFile ProfilePicture { get; set; }

        public string News { get; set; }

        [Display(Name = "Select Movies")]
        public List<int> SelectedMovieIds { get; set; } = new(); // List of movie IDs

        public List<SelectListItem> Movies { get; set; } = new();
    }
}
