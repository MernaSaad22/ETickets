using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ETickets.ViewModels
{
    public class CinemaCreateViewModel
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Display(Name = "Logo URL")]
        public string CinemaLogo { get; set; }

        public string Address { get; set; }

        [Display(Name = "Select Movies")]
        public List<int> SelectedMovieIds { get; set; } = new();

        public List<SelectListItem> Movies { get; set; } = new();
    }
}
