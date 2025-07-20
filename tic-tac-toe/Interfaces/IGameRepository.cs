using tic_tac_toe.Models;

namespace tic_tac_toe.Interfaces
{
    public interface IGameRepository
    {
        GameData Get(string id);
        void Save(GameData game);
        IEnumerable<GameData> GetAll();
    }
}