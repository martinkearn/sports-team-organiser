namespace STO.Models
{
    public abstract class Enums {
        public enum SortPagsBy { Name, Position, Team, GameBalance, PlayerBalance, Playing, Played };
        public enum SortTransactionsBy { Date, Amount, Player, Game };
        public enum Team { A, B };
        public enum PlayerPosition { Goalie,Defender,PlayMaker,BoxToBox,Midfielder,Forward }
        public enum SortRatingsBy { Rating, Player, Game };

        public enum EntityType
        {
            PlayerEntity,
            GameEntity,
            PlayerAtGameEntity,
            TransactionEntity,
            RatingEntity
        };

        public enum TitleLength
        {
            Short,
            Long
        };

        public enum PlayingStatus
        {
            Yes,
            Maybe,
            Reserve1,
            Reserve2,
            Reserve3,
            Reserve4,
            Reserve5,
            No
        }
    }
}