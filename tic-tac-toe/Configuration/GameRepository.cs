using MongoDB.Driver;
using tic_tac_toe.Models;

namespace tic_tac_toe.Configuration
{
    public class GameRepository
    {
        private readonly IMongoCollection<GameData> _games;

        public GameRepository(IConfiguration config)
        {
            var connectionString = config["MONGODB_CONNECTION_STRING"] ?? "mongodb://localhost:27017";
            var databaseName = config["MONGODB_DATABASE"] ?? "TicTacToeDb";

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _games = database.GetCollection<GameData>("Games");
        }

        public async Task CreateAsync(GameData game)
        {
            await _games.InsertOneAsync(game);
        }

        public async Task<GameData> GetByIdAsync(Guid id)
        {
            return await _games.Find(g => g.Id == id).FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(GameData game)
        {
            await _games.ReplaceOneAsync(g => g.Id == game.Id, game);
        }
    }
}
