using System.Threading.Tasks;
using System.Collections.Generic;
using TicTacToe.Common.Resources;
using TicTacToe.Core.Models;

namespace TicTacToe.Core.Interfaces.Repositories
{
    public interface IGameRepository
    {
        Task<List<Game>> GetGames();
        Task<List<Move>> GetMoves();
        Task AddGameResultAsync(GameResult result, List<string> movesHistory);
    }
}