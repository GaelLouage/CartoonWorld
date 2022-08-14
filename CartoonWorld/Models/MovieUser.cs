namespace CartoonWorld.Models
{
    public class MovieUser
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
  
        public Movie Movie { get; set; }
        public User Users { get; set; }
        public int UserId { get; set; }

    }
}
