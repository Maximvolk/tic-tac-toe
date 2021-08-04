using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TicTacToe.Core.Interfaces.Repositories;
using TicTacToe.Common.Resources;
using TicTacToe.Core.Models;

namespace TicTacToe.Persistence.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly TicTacToeContext _context;

        public GameRepository(TicTacToeContext context)
        {
            _context = context;
        }

        public async Task<List<Game>> GetGames()
        => await _context.Games.ToListAsync();

        public async Task<List<Move>> GetMoves()
        => await _context.Moves.ToListAsync();

        public async Task AddGameResultAsync(GameResult result, List<string> movesHistory)
        {
            var game = new Game { Result = result };

            await _context.Games.AddAsync(game);
            await _context.SaveChangesAsync();

            await _context.Moves.AddRangeAsync(movesHistory.Select(
                i => new Move { Coordinate = i, GameId = game.Id}));

            await _context.SaveChangesAsync();
        }
    }
}