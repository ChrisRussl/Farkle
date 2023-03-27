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

            Dice[] dices = new Dice[6];
            for (int i = 0; i < 6; i++)
            {
                dices[i] = new Dice();
            }

            // Initialize the score
            int score = 0;

            bool isGameOver = false;
            // Play the game
            while (!isGameOver)
            {
                //Initialize the dice
                
                foreach (Player player in scoreBoard.playerList)
                {
                    player.RunningScore = 0;

                    Console.WriteLine();

                    //player.ToString() returns the Player Id and accumulated score in
                    // $"Player {id} : {totalScore} Points"; format
                    Console.WriteLine(player);

                    //Console.Write("Roll the dice? (y/n) ");
                    //var key = Console.ReadKey();
                    //if (key.Key == ConsoleKey.N)
                    //    break;

                    //Console.WriteLine();

                    bool isTurnOver = false;
                    //Dice can be rolled multiple times

                    foreach (var dice in dices)
                    {
                        dice.SetPlayable();
                    }
                    //roll the dice until the player's turn is over
                    while (!isTurnOver)
                    {
                        Console.Write($"Player {player.id} : Roll the dice! Press any key to start!");
                        var key = Console.ReadKey();
                        //if (key.Key == ConsoleKey.N)
                        //    break;

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
                        ScoreDice(ref dices, player);

                        Console.WriteLine($"Your running score: {player.RunningScore}");
                        
                        //If the running score is not over  500, and the player has not been registered yet
                     //   if (!player.isRegistered) {
                            //Farkle, the player can't accumulate the score and the turn has to be tossed
                            if (player.RunningScore == 0)
                            {
                                Console.WriteLine($"Farkle on Player {player.id}. Not registered on the scoreboard.  No Points Scored.");
                                isTurnOver = true;
                            }
                            else if(player.RunningScore >= 500)
                            {
                            //if there are one or more dice the user can roll the dice

                            resetDice(ref dices);
                            Console.WriteLine($"Current number of rollable dice : {dices.Where((dice) => !dice.IsScored).Count()}");

                            Console.Write("\nWould you like to add your score(Ends turn)?: (Y/N) ");
                                    key = Console.ReadKey();
                                    Console.WriteLine();
                                    if (key.Key == ConsoleKey.Y)
                                    {   //🤡                                 
                                        player.totalScore += player.RunningScore;
                                        player.RunningScore = 0;
                                        isTurnOver = true;
                                    }
                                    else if (key.Key == ConsoleKey.N)
                                    {
                                        continue;
                                    }                                   
                               

                            }

                        //}
                        
                   }

                    // Check if the game is over
                    if (score >= 10000)
                    {
                        Console.WriteLine();
                        Console.WriteLine($"Player {player.id} wins!");
                        isGameOver = true;
                        break;
                    }
                }
            }
        }

        static void resetDice(ref Dice[] die) 
        {
            bool allScored = true;
            foreach (Dice d in die) 
            {
                if (!d.IsScored) 
                {
                    allScored = false;
                }
            }
            if (allScored) 
            {
                foreach (Dice d in die) 
                {
                    d.SetPlayable();
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

        //a function to determine if you can re-roll the current unrolled dice 
        //static bool DiceRollable(Dice[] dice){            
        //    int unscoredDiceCount = 0;

        //    foreach(Dice d in dice){

        //    }

        //    return true;
        //}

    static void ScoreDice(ref Dice[] die, Player player) 
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
                player.RunningScore += 3000;
                return;
            }
            if (numTriples == 2) 
            {
                player.RunningScore += 2500;
                return;
            }
            if (FiveOfAKind > 0) 
            {
                player.RunningScore += 2000;
                //This loop removes scored dice
                //Not needed for Six of a Kind or 2 Triples
                for (int i = 0; i < 5; ) 
                {
                    foreach (Dice d in die) 
                    {
                        if (!d.IsScored) 
                        {
                            d.SetAside();
                            ++i;
                            break;
                        }
                    }
                }
                
                if (Counts[1] == 1) 
                {
                    player.RunningScore += 100;
                    //This loop removes scored dice
                    //Not needed for Six of a Kind or 2 Triples
                    for (int i = 0; i < 1;)
                    {
                        foreach (Dice d in die)
                        {
                            if (!d.IsScored)
                            {
                                d.SetAside();
                                ++i;
                                break;
                            }
                        }
                    }
                }
                if (Counts[5] == 1)
                {
                    player.RunningScore += 50;
                    //This loop removes scored dice
                    //Not needed for Six of a Kind or 2 Triples
                    for (int i = 0; i < 1;)
                    {
                        foreach (Dice d in die)
                        {
                            if (!d.IsScored)
                            {
                                d.SetAside();
                                ++i;
                                break;
                            }
                        }
                    }
                }

                return;
            }
            if (FourOfAKind > 0)
            {
                player.RunningScore += 1000;
                //This loop removes scored dice
                //Not needed for Six of a Kind or 2 Triples
                for (int i = 0; i < 4;)
                {
                    foreach (Dice d in die)
                    {
                        if (!d.IsScored)
                        {
                            d.SetAside();
                            ++i;
                            break;
                        }
                    }
                }
                if (numPairs >0) 
                {
                    player.RunningScore += 500;
                    //This loop removes scored dice
                    //Not needed for Six of a Kind or 2 Triples
                    for (int i = 0; i < 2;)
                    {
                        foreach (Dice d in die)
                        {
                            if (!d.IsScored)
                            {
                                d.SetAside();
                                ++i;

                                break;
                            }
                        }
                    }
                    return;
                }
                if (Counts[1] == 1)
                {
                    player.RunningScore += 100;
                    //This loop removes scored dice
                    //Not needed for Six of a Kind or 2 Triples
                    for (int i = 0; i < 1;)
                    {
                        foreach (Dice d in die)
                        {
                            if (!d.IsScored)
                            {
                                d.SetAside();
                                ++i;
                                break;
                            }
                        }
                    }
                }
                if (Counts[5] == 1)
                {
                    player.RunningScore += 50;
                    //This loop removes scored dice
                    //Not needed for Six of a Kind or 2 Triples
                    for (int i = 0; i < 1;)
                    {
                        foreach (Dice d in die)
                        {
                            if (!d.IsScored)
                            {
                                d.SetAside();
                                ++i;
                                break;
                            }
                        }
                    }
                }
                return;
            }
            if (numTriples > 0) 
            {
                if (Counts[1] == 3) 
                {
                    player.RunningScore += 300;
                }
                for (int i = 2; i < Counts.Length; ++i) 
                {
                    if (Counts[i] == 3) 
                    {
                        player.RunningScore += i * 100;
                    }
                }
                //This loop removes scored dice
                //Not needed for Six of a Kind or 2 Triples
                for (int i = 0; i < 3;)
                {
                    foreach (Dice d in die)
                    {
                        if (!d.IsScored)
                        {
                            d.SetAside();
                            ++i;
                            break;
                        }
                    }
                }
                if (Counts[1] > 0 && Counts[1] != 3) 
                {
                    player.RunningScore += 100 * Counts[1];
                    //This loop removes scored dice
                    //Not needed for Six of a Kind or 2 Triples
                    for (int i = 0; i < Counts[1];)
                    {
                        foreach (Dice d in die)
                        {
                            if (!d.IsScored)
                            {
                                d.SetAside();
                                ++i;
                                break;
                            }
                        }
                    }
                }
                if (Counts[5] > 0 && Counts[5] != 3)
                {
                    player.RunningScore += 50 * Counts[5];
                    //This loop removes scored dice
                    //Not needed for Six of a Kind or 2 Triples
                    for (int i = 0; i < Counts[5];)
                    {
                        foreach (Dice d in die)
                        {
                            if (!d.IsScored)
                            {
                                d.SetAside();
                                ++i;
                                break;
                            }
                        }
                    }
                }
                return;
            }
            if (numPairs == 3) 
            {
                player.RunningScore += 1500;
                return;
            }

            if (Counts[1] == 1 && Counts[2] == 1 && Counts[3] == 1 && Counts[4] == 1 && Counts[5] == 1 && Counts[6] == 1) 
            {
                player.RunningScore += 1500;
                return;
            }

            //Farkle!
            if ((Counts[1] == 0 && Counts[5] == 0)) 
            {
                player.RunningScore = 0;
                Console.WriteLine("Farkle!!");
                return;
            }

            if (Counts[1] > 0) 
            {
                player.RunningScore += 100 * Counts[1];

                //This loop removes scored dice
                //Not needed for Six of a Kind or 2 Triples
                for (int i = 0; i < Counts[1];)
                {
                    foreach (Dice d in die)
                    {
                        if (!d.IsScored)
                        {
                            d.SetAside();
                            ++i;
                            break;
                        }
                    }
                }
            }

            if (Counts[5] > 0)
            {
                player.RunningScore += 50 * Counts[5];

                //This loop removes scored dice
                //Not needed for Six of a Kind or 2 Triples
                for (int i = 0; i < Counts[5];)
                {
                    foreach (Dice d in die)
                    {
                        if (!d.IsScored)
                        {
                            d.SetAside();
                            ++i;
                            break;
                        }
                    }
                }
            }

           
        }
    }
}