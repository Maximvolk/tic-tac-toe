using System.Threading.Tasks;
using TicTacToe.Common.Responses;
using TicTacToe.Common.Resources;

namespace TicTacToe.Core.Interfaces.Services
{
    public interface IGameService
    {
         Task<MoveResponse> Move(string userCoordinate, string userId);
         Task<GameResource[]> GetGamesHistory();
    }
}