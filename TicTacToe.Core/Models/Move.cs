namespace TicTacToe.Core.Models
{
    // By requirements moves are store in separate table
    // But as we add them with one query by the end of the game (for performance reasons)
    // and retrieve also with one query
    // it is reasonable to serialise them to string (json-like) and store in one table with games
    public class Move
    {
        public int Id { get; set; }
        public string Coordinate { get; set; }

        public int GameId { get; set; }
        public Game Game { get; set; }
    }
}