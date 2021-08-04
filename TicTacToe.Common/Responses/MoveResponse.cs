using TicTacToe.Common.Resources;

namespace TicTacToe.Common.Responses
{
    public class MoveResponse
    {
        public string ComputerCoordinate { get; set; }
        public GameResult Result { get; set; }
    }
}