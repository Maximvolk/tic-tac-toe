using System.Collections.Generic;
using TicTacToe.Common.Resources;

namespace TicTacToe.Core.Models
{
    public class Game
    {
        public int Id { get; set; }
        public GameResult Result { get; set; }
        public IList<Move> Moves { get; set; }

        // I would rather store moves here in some kind of json field
    }
}