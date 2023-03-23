namespace Farkle
{
    public class Player
    {
        public int id { get; set; }
        public int runningScore { get; set; }  = 0;
        public int totalScore { get; set; } = 0;
        
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