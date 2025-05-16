using ETickets.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ETickets.ViewModels
{
    public class CinemaFormViewModel
    {
        //public string Name { get; set; }
        //public string Description { get; set; }
        //public string Address { get; set; }
        //public IFormFile CinemaLogo { get; set; }

        //public List<int> SelectedMovieIds { get; set; } = new();
        //public List<SelectListItem> AvailableMovies { get; set; }
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        public string Description { get; set; }

        [Display(Name = "Cinema Logo")]
        public IFormFile ?LogoFile { get; set; }
        public string ?CinemaLogo { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }

        public List<int>? MovieIds { get; set; }
        public List<Movie>? AvailableMovies { get; set; }
    }
}
