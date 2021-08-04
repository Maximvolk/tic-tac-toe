using System;
using System.Runtime.Caching;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using TicTacToe.Core.Interfaces;

namespace TicTacToe.Core.Infrastructure
{
    public class GamesCollection : IGamesCollection
    {
        // MemoryCache was chosen over ConcurrentDictionary because of need
        // in expiration logic (to avoid stashing idle games which will never be finished)
        private readonly MemoryCache _games;
        private readonly MemoryCache _movesHistory;
        private readonly GameOptions _options;

        public GamesCollection(IOptions<GameOptions> options)
        {
            _games = new MemoryCache("games");
            _movesHistory = new MemoryCache("movesHistory");
            _options = options.Value;
        }

        public string[,] GetUserGame(string userId)
        => (string[,])_games.Get(userId) ?? new string[3,3];

        public void SetUserGame(string userId, string[,] game)
        => _games.Set(userId, game, DateTimeOffset.Now.AddHours(_options.GameTTL));

        public void AddMoveToHistory(string userId, string coordinate)
        {
            var moves = (List<string>)_movesHistory.Get(userId);

            if (moves == null)
                moves = new List<string>();

            moves.Add(coordinate);
            _movesHistory.Set(userId, moves, DateTimeOffset.Now.AddHours(_options.GameTTL));
        }

        // Assume movesHistory never empty by the moment of query
        // because we add move to history in the beginning
        public List<string> GetMovesHistory(string userId)
        => (List<string>)_movesHistory.Get(userId);

        public void RemoveUserGame(string userId)
        => _games.Remove(userId);

        public void RemoveUserHistory(string userId)
        => _movesHistory.Remove(userId);
    }
}