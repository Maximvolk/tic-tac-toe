using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TicTacToe.Core.Interfaces.Services;
using TicTacToe.Core.Interfaces.Repositories;
using TicTacToe.Core.Interfaces;
using TicTacToe.Core.Infrastructure;
using TicTacToe.Common.Resources;
using TicTacToe.Common.Responses;

namespace TicTacToe.Core.Services
{
    public class GameService : IGameService
    {
        private readonly IGameRepository _gameRepository;
        private readonly ILogger _logger;
        private readonly IGamesCollection _games;
        private readonly GameOptions _options;

        public GameService(
            IGameRepository gameRepository, ILogger<GameService> logger,
            IGamesCollection games, IOptions<GameOptions> options)
        {
            _gameRepository = gameRepository;
            _logger = logger;
            _games = games;
            _options = options.Value;
        }

        public async Task<GameResource[]> GetGamesHistory()
        {
            // To avoid "se;lect n + 1" problem we extract games with corresponding moves separately
            // And then process its join in memory
            var games = await _gameRepository.GetGames();
            var moves = await _gameRepository.GetMoves();

            // This (all method) will be much easier and faster if
            // moves history is serialised to string and stored in Games table in separate column
            return games.Select(g =>
                new GameResource
                    {
                        Id = g.Id,
                        Result = g.Result,
                        MovesHistory = string.Join(
                            ",", moves.Where(m => m.GameId == g.Id).Select(m => m.Coordinate))
                    }).ToArray();
        }

        public async Task<MoveResponse> Move(string userCoordinate, string userId)
        {
            _games.AddMoveToHistory(userId, userCoordinate);
            string[,] game = _games.GetUserGame(userId);

            // Add user's move to the board
            var userSystemCoordinates = _options.WebCoordinatesToSystem[userCoordinate];
            game[userSystemCoordinates[0], userSystemCoordinates[1]] = _options.UserSymbol;

            var gameResult = GetGameResult(game);

            // Clear data if game is already finished by user's move
            if (gameResult == GameResult.Draw || gameResult == GameResult.UserWon)
            {
                _logger.LogInformation($"#{userId} player game finished. Result: {gameResult}");
                await _gameRepository.AddGameResultAsync(gameResult, _games.GetMovesHistory(userId));
                ClearGameData(userId);

                return new MoveResponse
                    {
                        ComputerCoordinate = string.Empty,
                        Result = gameResult
                    };
            }

            // Compute computer's move and add it to the board
            var computerSystemCoordinates = FindBestMove(game);
            game[computerSystemCoordinates.Item1, computerSystemCoordinates.Item2] = _options.ComputerSymbol;

            var computerCoordinate = SystemCoordinatesToWeb(computerSystemCoordinates);
            _games.AddMoveToHistory(userId, computerCoordinate);
            _games.SetUserGame(userId, game);

            gameResult = GetGameResult(game);
        
            // Clear game data if game is finished
            if (gameResult != GameResult.GameNotFinished)
            {
                _logger.LogInformation($"#{userId} player game finished. Result: {gameResult}");
                await _gameRepository.AddGameResultAsync(gameResult, _games.GetMovesHistory(userId));
                ClearGameData(userId);
            }

            return new MoveResponse
                    {
                        ComputerCoordinate = computerCoordinate,
                        Result = gameResult
                    };
        }

        private GameResult GetGameResult(string[,] game)
        {
            var gameScore = EvaluateGame(game);

            if (gameScore == 0 && !AreMovesLeft(game))
                return GameResult.Draw;

            if (gameScore < 0)
                return GameResult.UserWon;
            else if (gameScore > 0)
                return GameResult.ComputerWon;
            else
                return GameResult.GameNotFinished;
        }

        private bool AreMovesLeft(string[,] game)
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (game[i, j] == null)
                        return true;

            return false;
        }

        private Tuple<int, int> FindBestMove(string[,] game)
        {
            int bestScore = -1000;
            int bestMoveX = -1;
            int bestMoveY = -1;
        
            // Traverse all cells, evaluate minimax function
            // for all empty cells. And return the cell
            // with optimal value.
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    // Check if cell is empty
                    if (game[i, j] == null)
                    {
                        // Make the move
                        game[i, j] = _options.ComputerSymbol;
                        int moveScore = Minimax(game, 0, false);
        
                        // Undo the move
                        game[i, j] = null;

                        if (moveScore > bestScore)
                        {
                            bestMoveX = i;
                            bestMoveY = j;
                            bestScore = moveScore;
                        }
                    }
                }
            }

            return new Tuple<int, int>(bestMoveX, bestMoveY);  
        }

        private int Minimax(string[,] game, int depth, bool isMax)
        {
            int score = EvaluateGame(game);

            if (score == 10)
                return score - depth;

            if (score == -10)
                return score + depth;

            if (AreMovesLeft(game) == false)
                return 0;
        
            // If this maximizer's move
            if (isMax)
            {
                int best = -1000;
        
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (game[i, j] == null)
                        {
                            // Make the move
                            game[i, j] = _options.ComputerSymbol;

                            best = Math.Max(best, Minimax(game,
                                            depth + 1, !isMax));
        
                            // Undo the move
                            game[i, j] = null;
                        }
                    }
                }
                return best;
            }
        
            // If this minimizer's move
            else
            {
                int best = 1000;
        
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (game[i, j] == null)
                        {
                            game[i, j] = _options.UserSymbol;

                            best = Math.Min(best, Minimax(game,
                                            depth + 1, !isMax));

                            game[i, j] = null;
                        }
                    }
                }
                return best;
            }    
        }

        private int EvaluateGame(string[,] game)
        {
            // Check for "row" victory
            for (int row = 0; row < 3; row++)
            {
                if (game[row, 0] == game[row, 1] &&
                    game[row, 1] == game[row, 2])
                {
                    if (game[row, 0] == _options.ComputerSymbol)
                        return +10;
                    else if (game[row, 0] == _options.UserSymbol)
                        return -10;
                }
            }
        
            // Checking "column" victory.
            for (int col = 0; col < 3; col++)
            {
                if (game[0, col] == game[1, col] &&
                    game[1, col] == game[2, col])
                {
                    if (game[0, col] == _options.ComputerSymbol)
                        return +10;
        
                    else if (game[0, col] == _options.UserSymbol)
                        return -10;
                }
            }
        
            // Checking for "diagonal" victory.
            if (game[0, 0] == game[1, 1] && game[1, 1] == game[2, 2])
            {
                if (game[0, 0] == _options.ComputerSymbol)
                    return +10;
                else if (game[0, 0] == _options.UserSymbol)
                    return -10;
            }
        
            if (game[0, 2] == game[1, 1] && game[1, 1] == game[2, 0])
            {
                if (game[0, 2] == _options.ComputerSymbol)
                    return +10;
                else if (game[0, 2] == _options.UserSymbol)
                    return -10;
            }
        
            return 0;
        }

        private void ClearGameData(string userId)
        {
            _games.RemoveUserGame(userId);
            _games.RemoveUserHistory(userId);
        }

        private string SystemCoordinatesToWeb(Tuple<int, int> systemCoordinates)
        {
            var systemCoordinateString = 
                systemCoordinates.Item1.ToString() +
                systemCoordinates.Item2.ToString();
            
            return _options.SystemCoordinatesToWeb[systemCoordinateString];
        }
    }
}