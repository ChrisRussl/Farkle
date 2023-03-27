namespace Farkle
{
    public class Player
    {
        public int id { get; set; }
        public int RunningScore { get; set; }  = 0;
        public int totalScore { get; set; } = 0;
        public bool isRegistered { get; set; } = false;
        public override string ToString()
        {
            return $"Player {id} : {totalScore} Points";
        }

        public Player(int id)
        {
            this.id = id;
        }
    }
}