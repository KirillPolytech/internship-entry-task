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
            GameData game = _gameService.CreateGame();
            return CreatedAtAction(nameof(GetGame), new { id = game.Id }, game);
        }

        [HttpGet("{id}/GetGame")]
        public IActionResult GetGame(string id)
        {
            GameData game = _gameService.GetGame(id);

            return game == null ? NotFound() : Ok(game);
        }

        [HttpPost("{gameId}/MakeMove")]
        public IActionResult MakeMove(Guid gameId, [FromBody] MoveRequest move)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(new ValidationProblemDetails(ModelState)
                {
                    Status = 400,
                    Type = "https://tools.ietf.org/html/rfc7807",
                    Title = "Invalid JSON request"
                });
            }

            var game = _gameService.GetGame(gameId.ToString());

            if (game == null) 
                return NotFound();

            var key = $"{move.Row}-{move.Col}-{game.CurrentPlayer}";

            if (game.MoveHistory.TryGetValue(key, out var cachedResult))
            {
                Response.Headers["ETag"] = cachedResult.ETag;
                return Ok(cachedResult.Response);
            }

            bool success = _gameService.MakeMove(game, move.Row, move.Col);

            var response = new MoveResponse
            {
                Success = success,
                CurrentPlayer = game.CurrentPlayer,
                Board = game.Board                
            };

            var etag = $"\"{Guid.NewGuid()}\"";

            game.MoveHistory[key] = new MoveResult
            {
                Response = response,
                ETag = etag
            };

            Response.Headers["ETag"] = etag;

            return Ok(response);
        }

        [HttpGet("{gameId}/GetGameState")]
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