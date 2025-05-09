using ETickets.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ETickets.ViewModels
{
    public class MovieEditViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
        public double Price { get; set; }

        // This field holds the URL of the existing image (if there's an existing image)
        public string ExistingImgUrl { get; set; }

        // This is for uploading a new image if the user chooses to
        public IFormFile? NewImgUrl { get; set; }

        // This field holds the URL of the existing trailer (if there's an existing trailer)
        public string ExistingTrailerUrl { get; set; }

        // This is for uploading a new trailer if the user chooses to
        public IFormFile? NewTrailerUrl { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public MovieStatus Status { get; set; }

        // The ID of the selected Cinema and Category
        public int CinemaId { get; set; }
        public int CategoryId { get; set; }

        // A list of actor IDs associated with the movie
        [Display(Name = "Select Actors")]
        public List<int> SelectedActorIds { get; set; } = new();

        // These are the available actors that can be selected (you'll populate this from the controller)
        public List<SelectListItem> Actors { get; set; } = new();

        // These are the available cinemas (you'll populate this from the controller)
        public List<SelectListItem> Cinemas { get; set; } = new();

        // These are the available categories (you'll populate this from the controller)
        public List<SelectListItem> Categories { get; set; } = new();

        // Optional: Movie status list to show in a dropdown
        public List<SelectListItem> MovieStatusList { get; set; } = new();
    }
}
