using tic_tac_toe.Models;
using Newtonsoft.Json;
using tic_tac_toe.Interfaces;

namespace tic_tac_toe.Services
{
    public class JsonGameRepositoryService : IGameRepository
    {
        private readonly string _folderPath = "data";

        public JsonGameRepositoryService()
        {
            if (!Directory.Exists(_folderPath))
                Directory.CreateDirectory(_folderPath);
        }

        public GameData Get(string id)
        {
            var path = Path.Combine(_folderPath, $"{id}.json");
            if (!File.Exists(path)) 
                return null;

            var json = File.ReadAllText(path);

            return JsonConvert.DeserializeObject<GameData>(json);
        }

        public void Save(GameData game)
        {
            var path = Path.Combine(_folderPath, $"{game.Id}.json");
            var json = JsonConvert.SerializeObject(game, Formatting.Indented);

            File.WriteAllText(path, json);
        }

        public IEnumerable<GameData> GetAll()
        {
            return Directory.GetFiles(_folderPath, "*.json")
                .Select(f => JsonConvert.DeserializeObject<GameData>(File.ReadAllText(f)));
        }
    }
}