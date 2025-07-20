using Newtonsoft.Json;

namespace tic_tac_toe.Models
{
    public class MoveResult
    {
        [JsonIgnore]
        public object Response { get; set; }
        public string ETag { get; set; }
    }
}