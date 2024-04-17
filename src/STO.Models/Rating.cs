namespace STO.Models
{
    public class Rating
    {
        public Rating(RatingEntity ratingEntity)
        {
            this.RatingEntity = ratingEntity;
        } 
        public RatingEntity RatingEntity { get; set; } = default!;
        
        public Player Player { get; set; } = default!;

        public Game Game { get; set; } = default!;
    }
}