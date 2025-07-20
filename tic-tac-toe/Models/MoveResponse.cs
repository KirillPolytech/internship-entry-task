namespace tic_tac_toe.Models
{
    public class MoveResponse
    {
        public bool Success { get; set; }

        public PlayerType CurrentPlayer { get; set; }
        public PlayerType[][] Board { get; set; }
    }
}