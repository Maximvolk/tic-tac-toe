using System.Collections.Generic;

namespace TicTacToe.Core.Infrastructure
{
    public class GameOptions
    {
        public Dictionary<string, int[]> WebCoordinatesToSystem { get; set; }
        public Dictionary<string, string> SystemCoordinatesToWeb { get; set; }

        public string UserSymbol { get; set; }
        public string ComputerSymbol { get; set; }

        // Maximum time (number of hours) game in progress will be stored
        public int GameTTL { get; set; }
    }
}