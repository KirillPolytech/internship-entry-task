using Microsoft.Extensions.Options;
using tic_tac_toe.Configuration;
using tic_tac_toe.Models;

namespace tic_tac_toe.Services
{
    public class GameService
    {
        private readonly Random _random = new();
        private readonly GameSettings _settings;

        public GameService(IOptions<GameSettings> settings)
        {
            _settings = settings.Value;
        }

        private PlayerType GetOpponent(PlayerType player) => player == PlayerType.X ? PlayerType.O : PlayerType.X;

        public GameData CreateGame()
        {
            return new GameData(_settings.FieldSize, _settings.WinCondition);
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

            // Проверяем правило для каждого третьего хода
            PlayerType symbolToPlace = game.CurrentPlayer;
            if (game.MoveCount % 3 == 0)
            {
                var chance = _random.NextDouble();
                if (chance <= 0.10) // 10%
                {
                    symbolToPlace = GetOpponent(game.CurrentPlayer);
                }
            }

            game.Board[row][col] = symbolToPlace;

            // Проверяем победу
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
                // Если ход сделан символом текущего игрока, меняем игрока
                if (symbolToPlace == game.CurrentPlayer)
                    game.CurrentPlayer = GetOpponent(game.CurrentPlayer);
            }

            return true;
        }

        private bool CheckWin(GameData game, int row, int col, PlayerType player)
        {
            int inRow = game.WinCondition;
            int n = game.FieldSize;

            // Проверяем в 4 направлениях: горизонталь, вертикаль, диагональ \, диагональ /

            bool CheckDirection(int dr, int dc)
            {
                int count = 1;

                // В одну сторону
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

                // В другую сторону
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

            return CheckDirection(0, 1)   // горизонталь
                || CheckDirection(1, 0)   // вертикаль
                || CheckDirection(1, 1)   // диагональ \
                || CheckDirection(1, -1); // диагональ /
        }
    }
}
