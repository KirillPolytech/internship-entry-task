using Microsoft.Extensions.Options;
using tic_tac_toe.Configuration;
using tic_tac_toe.Interfaces;
using tic_tac_toe.Models;
using tic_tac_toe.Services;
using Xunit;

namespace TicTacToe.Tests.Unit;

public class UnitTest1
{
    private GameService CreateService(int fieldSize = 3, int winCondition = 3)
    {
        var settings = Options.Create(new GameSettings
        {
            FieldSize = fieldSize,
            WinCondition = winCondition
        });

        IGameRepository repository = new JsonGameRepositoryService();
        return new GameService(settings, repository);
    }

    [Fact]
    public void CreateGame_ReturnsInitializedGameData()
    {
        var service = CreateService();
        var game = service.CreateGame();

        Assert.Equal(3, game.FieldSize);
        Assert.Equal(3, game.WinCondition);
        Assert.Equal(GameStatus.InProgress, game.Status);
        Assert.All(game.Board, row => Assert.All(row, cell => Assert.Equal(PlayerType.None, cell)));
    }

    [Fact]
    public void MakeMove_ValidMove_ChangesBoardAndPlayer()
    {
        var service = CreateService();
        var game = service.CreateGame();

        bool result = service.MakeMove(game, 0, 0);

        Assert.True(result);
        Assert.NotEqual(PlayerType.None, game.Board[0][0]);
    }

    [Fact]
    public void MakeMove_InvalidOutOfBounds_ReturnsFalse()
    {
        var service = CreateService();
        var game = service.CreateGame();

        bool result = service.MakeMove(game, 10, 10);

        Assert.False(result);
    }

    [Fact]
    public void MakeMove_CellOccupied_ReturnsFalse()
    {
        var service = CreateService();
        var game = service.CreateGame();

        service.MakeMove(game, 0, 0);
        bool result = service.MakeMove(game, 0, 0);

        Assert.False(result);
    }

    [Fact]
    public void MakeMove_TriggersWin_UpdatesStatus()
    {
        var service = CreateService();
        var game = service.CreateGame();

        service.MakeMove(game, 0, 0); // X
        service.MakeMove(game, 1, 0); // O
        service.MakeMove(game, 0, 1); // X
        service.MakeMove(game, 1, 1); // O
        service.MakeMove(game, 0, 2); // X (Win)

        Assert.Equal(GameStatus.Won, game.Status);
        Assert.Equal(PlayerType.X, game.Winner);
    }

    [Fact]
    public void MakeMove_Draw_UpdatesStatus()
    {
        var service = CreateService();
        var game = service.CreateGame();

        // Board:
        // X O X
        // X O O
        // O X X

        service.MakeMove(game, 0, 0); // X
        service.MakeMove(game, 0, 1); // O
        service.MakeMove(game, 0, 2); // X
        service.MakeMove(game, 1, 1); // O
        service.MakeMove(game, 1, 0); // X
        service.MakeMove(game, 1, 2); // O
        service.MakeMove(game, 2, 1); // X
        service.MakeMove(game, 2, 0); // O
        service.MakeMove(game, 2, 2); // X

        Assert.Equal(GameStatus.Draw, game.Status);
    }

    [Fact]
    public void MakeMove_ThirdMove_HasChanceToPlaceOpponent()
    {
        var service = CreateService();
        var game = service.CreateGame();

        service.MakeMove(game, 0, 0); // 1
        service.MakeMove(game, 1, 0); // 2
        service.MakeMove(game, 0, 1); // 3

        var symbol = game.Board[0][1];
        Assert.True(symbol == PlayerType.X || symbol == PlayerType.O);
    }
}