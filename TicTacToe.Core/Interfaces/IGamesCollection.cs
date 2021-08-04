using System.Collections.Generic;

namespace TicTacToe.Core.Interfaces
{
    public interface IGamesCollection
    {
        string[,] GetUserGame(string userId);
        void SetUserGame(string userId, string[,] game);
        void AddMoveToHistory(string userId, string coordinate);
        List<string> GetMovesHistory(string userId);
        void RemoveUserGame(string userId);
        void RemoveUserHistory(string userId);
    }
}