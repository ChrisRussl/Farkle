//Author : Hongseok Kim (Harry)
using System.Collections.Generic;
namespace Farkle
{
    public class ScoreBoard
    {
        public List<Player> playerList = new List<Player>();

        public ScoreBoard(int playerNumber)
        {
            if(playerNumber < 2 || playerNumber > 8) {
                Console.WriteLine("Failed to assign the players. The number of Players must be within range of 2~8.");
                return;
            }
            for(int i = 1; i <= playerNumber; i++)
            {
                playerList.Add(new Player(i));
            }
        }

        public void PrintScoreboard()
        {
            foreach(Player player in playerList)
            {
                Console.WriteLine(player);
            }
        }

        public void UpdateScore(int playerId, int score)
        {
            //not sure if this is right, make sure you test this line in future
            playerList.Where((player)=>player.id== playerId).First().totalScore = score;
        }
    }
}