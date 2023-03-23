using System;
using System.Collections.Generic;

namespace Farkle
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Farkle!");

            //get the number of players
            Console.WriteLine();
            Console.Write("Enter the number of players (2-8): ");
            int numOfPlayers = 0;

            while (numOfPlayers < 2 || numOfPlayers > 8)
            {
                Console.Write("Enter the number of players (2-8): ");

                try
                {
                    numOfPlayers = Convert.ToInt32(Console.ReadLine());
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Invalid input. Please enter a number between 2 and 8.");
                }
            }
            Console.WriteLine();

            ScoreBoard scoreBoard = new ScoreBoard(numOfPlayers);

            // Initialize the dice
            Dice[] dices = new Dice[6];

            // Initialize the score
            int score = 0;

            // Play the game
            while (true)
            {
                foreach (Player player in scoreBoard.playerList)
                {
                    player.runningScore = 0;

                    Console.WriteLine();

                    //player.ToString() returns the Player Id and accumulated score in
                    // $"Player {id} : {totalScore} Points"; format
                    Console.WriteLine(player);

                    Console.Write("Roll the dice? (y/n) ");
                    var key = Console.ReadKey();
                    if (key.Key == ConsoleKey.N)
                        break;

                    Console.WriteLine();

                    // Roll the dice
                    foreach (Dice d in dices)
                    {
                        if (!d.IsScored)
                            d.Roll();
                    }

                    // Display the dice
                    DisplayDice(dices);

                    // Determine the score for this roll
                    //score += CalculateScore(dice);
                    ScoreDice(dices, player.runningScore);

                    // Check if the game is over
                    if (score >= 10000)
                    {
                        Console.WriteLine();
                        Console.WriteLine("You win!");
                        break;
                    }
                }
            }
        }


        static void DisplayDice(Dice[] die)
        {
            foreach (Dice d in die) {
                if (!d.IsScored)
                {
                    Console.WriteLine("Dice: " + d.Value);
                }
            }
            
        }

        static void ScoreDice(Dice[] die, int RunningScore) 

        {

            int[] Counts = new int[7]; //0 Won't be used
            foreach (Dice d in die) 
            {
                if (!d.IsScored) 
                {
                    Counts[d.Value]++;
                }
            }
          
            int SixOfAKind = 0;
            int FiveOfAKind = 0;
            int FourOfAKind = 0;
            int numTriples = 0;
            int numPairs = 0;

            for (int i = 0; i < Counts.Length; i++)
            {
                if (Counts[i] == 6)
                {
                    SixOfAKind++; 
                }
                if (Counts[i] == 5)
                {
                    FiveOfAKind++;
                }
                if (Counts[i] == 4)
                {
                    FourOfAKind++;
                }
                if (Counts[i] == 3)
                {
                    numTriples++;                    
                }
                if (Counts[i] == 2)
                {
                    numPairs++;
                }
            }

            if (SixOfAKind > 0) 
            {
                RunningScore += 3000;
                return;
            }
            if (numTriples == 2) 
            {
                RunningScore = +2500;
                return;
            }
            if (FiveOfAKind > 0) 
            {
                RunningScore += 2000;
                if (Counts[1] == 1) 
                {
                    RunningScore += 100;
                }
                if (Counts[5] == 1)
                {
                    RunningScore += 50;
                }

                return;
            }
            if (FourOfAKind > 0)
            {
                RunningScore += 1000;
                if (numPairs >0) 
                {
                    RunningScore += 500;
                    return;
                }
                if (Counts[1] == 1)
                {
                    RunningScore += 100;
                }
                if (Counts[5] == 1)
                {
                    RunningScore += 50;
                }
                return;
            }
            if (numTriples > 0) 
            {
                if (Counts[1] == 3) 
                {
                    RunningScore += 300;
                }
                for (int i = 2; i < Counts.Length; ++i) 
                {
                    if (Counts[i] == 3) 
                    {
                        RunningScore += i * 100;
                    }
                }
                if (Counts[1] > 0 && Counts[1] != 3) 
                {
                    RunningScore += 100 * Counts[1];
                }
                if (Counts[5] > 0 && Counts[5] != 3)
                {
                    RunningScore += 50 * Counts[5];
                }
                return;
            }
            if (numPairs == 3) 
            {
                RunningScore += 1500;
                return;
            }

            if (Counts[1] == 1 && Counts[2] == 1 && Counts[3] == 1 && Counts[4] == 1 && Counts[5] == 1 && Counts[6] == 1) 
            {
                RunningScore += 1500;
                return;
            }

            //Farkle!
            if ((Counts[1] == 0 && Counts[5] == 0)) 
            {
                RunningScore = 0;
                return;
            }

            if (Counts[1] > 0) 
            {
                RunningScore += 100 * Counts[1];
            }

            if (Counts[5] > 0)
            {
                RunningScore += 50 * Counts[5];
            }

           
        }
    }
}