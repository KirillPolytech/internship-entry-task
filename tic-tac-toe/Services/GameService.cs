using Microsoft.Extensions.Options;
using tic_tac_toe.Configuration;
using tic_tac_toe.Interfaces;
using tic_tac_toe.Models;

namespace tic_tac_toe.Services
{
    public class GameService
    {
        private readonly Random _random = new();
        private readonly GameSettings _settings;
        private readonly IGameRepository _gameRepository;
        private readonly double _errorChance = 0.1d;

        public GameService(IOptions<GameSettings> settings, IGameRepository gameRepository)
        {
            _settings = settings.Value;
            _gameRepository = gameRepository;
        }

        private PlayerType GetOpponent(PlayerType player) => player == PlayerType.X ? PlayerType.O : PlayerType.X;

        public GameData CreateGame()
        {
            GameData game = new GameData(_settings.FieldSize, _settings.WinCondition);
            _gameRepository.Save(game);
            return game;
        }

        public bool MakeMove(GameData game, int row, int col)
        {
            if (game.Status != GameStatus.InProgress)
                return false;

            if (row < 0 || row >= game.FieldSize || col < 0 || col >= game.FieldSize)
                return false;

            if (game.Board[row][col] != PlayerType.None)
                return false;

            game.MoveCount++;

            PlayerType symbolToPlace = game.CurrentPlayer;
            if (game.MoveCount % 3 == 0)
            {
                var chance = _random.NextDouble();
                if (chance <= _errorChance)
                {
                    symbolToPlace = GetOpponent(game.CurrentPlayer);
                }
            }

            game.Board[row][col] = symbolToPlace;

            if (CheckWin(game, row, col, symbolToPlace))
            {
                game.Status = GameStatus.Won;
                game.Winner = symbolToPlace;
            }
            else if (game.MoveCount == game.FieldSize * game.FieldSize)
            {
                game.Status = GameStatus.Draw;
            }
            else
            {
                if (symbolToPlace == game.CurrentPlayer)
                    game.CurrentPlayer = GetOpponent(game.CurrentPlayer);
            }

            _gameRepository.Save(game);

            return true;
        }

        private bool CheckWin(GameData game, int row, int col, PlayerType player)
        {
            bool result =
                CheckDirection(game, 0, 1, player, row, col)
                || CheckDirection(game, 1, 0, player, row, col)
                || CheckDirection(game, 1, 1, player, row, col)
                || CheckDirection(game, 1, -1, player, row, col);

            _gameRepository.Save(game);

            return result;
        }

        public GameData GetGame(string id)
        {
            return _gameRepository.Get(id);
        }

        private bool CheckDirection(GameData game, int dr, int dc, PlayerType player, int row, int col)
        {
            int inRow = game.WinCondition;
            int n = game.FieldSize;

            int count = 1;

            for (int i = 1; i < inRow; i++)
            {
                int r = row + dr * i;
                int c = col + dc * i;
                if (r < 0 || r >= n || c < 0 || c >= n)
                    break;
                if (game.Board[r][c] == player)
                    count++;
                else
                    break;
            }

            for (int i = 1; i < inRow; i++)
            {
                int r = row - dr * i;
                int c = col - dc * i;

                if (r < 0 || r >= n || c < 0 || c >= n)
                    break;

                if (game.Board[r][c] == player)
                    count++;
                else
                    break;
            }

            return count >= inRow;
        }
    }
}