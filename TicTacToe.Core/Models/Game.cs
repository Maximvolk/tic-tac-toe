namespace TicTacToe.Core.Models
{
    public class Game
    {
        public int Id { get; set; }
        // True if player won the game false if computer won
        public bool UserWon { get; set; }
    }
}