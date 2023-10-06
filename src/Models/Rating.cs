namespace STO.Models
{
    public class Rating
    {
        public Rating(RatingEntity RatingEntity)
        {
            this.RatingEntity = RatingEntity;
        } 
        public RatingEntity RatingEntity { get; set; } = default!;
        
        public Player Player { get; set; } = default!;

        public Game Game { get; set; } = default!;
    }
}