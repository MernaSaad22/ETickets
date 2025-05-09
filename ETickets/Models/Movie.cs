namespace ETickets.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; }
        public double Price { get; set; }
        public string ImgUrl { get; set; } = null!;
        public string TrailerUrl { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public MovieStatus Status { get; set; }
        public int CinemaId { get; set; }
        public int CategoryId { get; set; }
        public Cinema Cinema { get; set; } = null!;
        public Category Category { get; set; } = null!;

       public ICollection<ActorMovie> ActorMovies { get; set; }



    }
}
