using ETickets.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ETickets.ViewModels
{
    public class MovieCreateViewModel
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        [Required]
        public IFormFile ImgUrl { get; set; } = null!;
        [Required]
        public IFormFile TrailerUrl { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public MovieStatus Status { get; set; }
        [Display(Name = "Cinema")]
        public int CinemaId { get; set; }
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        [Display(Name = "Actors")]
        public List<int> ActorIds { get; set; }
    }
}
