namespace tic_tac_toe.Models
{
    public class GameData
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public PlayerType[][] Board { get; set; }
        public PlayerType CurrentPlayer { get; set; } = PlayerType.X;
        public int MoveCount { get; set; } = 0;
        public GameStatus Status { get; set; } = GameStatus.InProgress;
        public PlayerType Winner { get; set; } = PlayerType.None;
        public int FieldSize { get; set; }
        public int WinCondition { get; set; }

        public GameData(int fieldSize, int winCondition)
        {
            FieldSize = fieldSize;
            WinCondition = winCondition;

            Board = new PlayerType[fieldSize][];
            for (int i = 0; i < fieldSize; i++)
                Board[i] = new PlayerType[fieldSize];           
        }
    }
}