using ETickets.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ETickets.ViewModels
{
    public class MovieCreateViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public IFormFile ImgUrl { get; set; } = null!;
        public IFormFile TrailerUrl { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public MovieStatus Status { get; set; }
        public int CinemaId { get; set; }
        public int CategoryId { get; set; }
        public List<int> ActorIds { get; set; }
    }
}
