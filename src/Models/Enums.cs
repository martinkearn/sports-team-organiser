namespace STO.Models
{
    public class Enums {
        public enum SortPagsBy { Name, Position, Team, GameBalance, PlayerBalance, Playing, Played };
        public enum SortTransactionsBy { Date, Amount, Player, Game };
        public enum Team { A, B };
        public enum PlayerPosition { Goalie,Defender,PlayMaker,BoxToBox,Midfielder,Forward }
    }
}