using Microsoft.AspNetCore.Mvc;
using tic_tac_toe.Models;
using tic_tac_toe.Services;

namespace tic_tac_toe.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameController : ControllerBase
    {
        private static readonly Dictionary<Guid, GameData> Games = new();
        private readonly GameService _gameService;

        public GameController(GameService gameService)
        {
            _gameService = gameService;
        }

        [HttpPost("CreateGame")]
        public IActionResult CreateGame()
        {
            var game = _gameService.CreateGame();
            Games[game.Id] = game;
            return Ok(new { gameId = game.Id });
        }

        [HttpPost("{gameId}/move")]
        public IActionResult MakeMove(Guid gameId, [FromBody] MoveRequest move)
        {
            if (!Games.TryGetValue(gameId, out var game))
                return NotFound("Game not found");

            bool success =  _gameService.MakeMove(game, move.Row, move.Col);

            if (!success)
                return BadRequest("Invalid move or game finished");

            return Ok(new
            {
                game.Status,
                game.Winner,
                CurrentPlayer = game.CurrentPlayer,
                Board = game.Board
            });
        }

        [HttpGet("{gameId}")]
        public IActionResult GetGameState(Guid gameId)
        {
            if (!Games.TryGetValue(gameId, out var game))
                return NotFound("Game not found");

            return Ok(new
            {
                game.Status,
                game.Winner,
                CurrentPlayer = game.CurrentPlayer,
                Board = game.Board
            });
        }
    }
}
