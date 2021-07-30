namespace TicTacToe.Core.Models
{
    public class Move
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public string Coordinate { get; set; }
    }
}