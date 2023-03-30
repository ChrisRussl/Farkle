/*
 * Program:   FarkleLibrary
 * Module:     Farkle.cs
 * Author:      Dustin Taylor, Hongseok Kim, Donghao Tang, Christopher Russell
 * Date:        March 30, 2023
 * Description: The implementation of a Farkle game, allowing users to join/leave the game or roll the dice.  
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ServiceModel;
using System.Threading;

namespace FarkleLibrary
{
    public interface ICallback
    {
        [OperationContract(IsOneWay = true)]
        void Update(int[] scores, int nextClient, bool gameOver);

        [OperationContract(IsOneWay = true)]
        void UpdateDiceRoll(string roll);

        [OperationContract(IsOneWay = true)]
        void UpdateGameStart(bool GameStart, int nextId);
    }

    [ServiceContract(CallbackContract = typeof(ICallback))]
    public interface IFarkle
    {
        [OperationContract]
        int JoinGame();

        [OperationContract(IsOneWay = true)]
        void LeaveGame();
        
        [OperationContract(IsOneWay = true)]
        void RollDice(int clientId);

        [OperationContract(IsOneWay = true)]
        void DisplayDice();
        
        [OperationContract(IsOneWay = false)]
        void ScoreDice(ref int runningScore);

        [OperationContract(IsOneWay = true)]
        void NextTurn();

        [OperationContract(IsOneWay = true)]
        void ResetDice();

        [OperationContract(IsOneWay = false)]
        string PlayableDice();

        [OperationContract(IsOneWay = true)]
        void UpdateScore(int score);
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Farkle : IFarkle
    {
        private int[] scores = new int[9]; // Keeps track of the game count for this counting "game"
        private readonly Dictionary<int, ICallback> callbacks = null;   // Stores Id and callback for each client
        private int nextClientId;                                       // Unique Id to be assigned of next client that "Joins"
        private int clientIndex;                                        // Index of client that "counts" next
        private bool gameOver, gameStart = false;
        private string roll;

        Dice[] dices = new Dice[6];


        public Farkle()
        {
            nextClientId = 1;
            clientIndex = 0;
            callbacks = new Dictionary<int, ICallback>();
            for (int i = 0; i < 6; i++)
            {
                dices[i] = new Dice();
            }
        }

        public string PlayableDice() 
        {
            return ($"Current number of rollable dice : {dices.Where((dice) => !dice.IsScored).Count()}");
        }
        public void NextTurn() 
        {
            clientIndex = ++clientIndex % callbacks.Count;

            foreach (Dice d in dices)
            {
                d.SetPlayable();
            }
            UpdateAllClients();
        }

        public void UpdateScore(int score) 
        {
            scores[clientIndex + 1] += score;
            if (scores[clientIndex + 1] > 2500) 
            {
                gameOver = true;
            }
            //UpdateAllClients();
        }

        public int JoinGame()
        {
            ICallback callback = OperationContext.Current.GetCallbackChannel<ICallback>();
            if (callbacks.ContainsValue(callback))
            {
                int i = callbacks.Values.ToList().IndexOf(callback);
                return callbacks.Keys.ElementAt(i);
            }

            //maximum player # is 8
            if (clientIndex < 8)
            {
                callbacks.Add(nextClientId, callback);
                if (nextClientId + 1 > 2) 
                {
                    gameStart = true;
                    UpdateAllClientsGameStart();
                }
                return nextClientId++;
            }
            //have to be implemented
            else return -1;
        }

        public void LeaveGame()
        {
            // Identify which client is calling this method
            ICallback cb = OperationContext.Current.GetCallbackChannel<ICallback>();

            if (callbacks.ContainsValue(cb))
            {
                // Identify which client is currently calling this method
                // - Get the index of the client within the callbacks collection
                int i = callbacks.Values.ToList().IndexOf(cb);
                // - Get the unique id of the client as stored in the collection
                int id = callbacks.ElementAt(i).Key;

                // Remove this client from receiving callbacks from the service
                callbacks.Remove(id);

                // Make sure the counting sequence isn't disrupted by removing this client
                if (i == clientIndex)
                    // This client was supposed to count next but is exiting the game
                    // Need to signal the next client to count instead 
                    UpdateAllClients();
                else if (clientIndex > i)
                    // This prevents a player from being "skipped over" in the turn-taking
                    // of this "game"
                    clientIndex--;
            }
        }

        public void RollDice(int clientId) 
        {
            if (clientId == clientIndex+1)
            {
                foreach (Dice d in dices)
                {
                    if (!d.IsScored)
                        d.Roll();
                    Console.WriteLine(d.Value);

                    var random = new Random();
                    //random.Next(1, 7);
                    System.Threading.Thread.Sleep(random.Next(5, 20));
                }
            }
        }

        public void DisplayDice()
        {
            roll = "";
            foreach (Dice d in dices)
            {
                if (!d.IsScored)
                {
                    roll += ("\nDice: " + d.Value);
                }
            }
            //clientIndex = ++clientIndex % callbacks.Count;
            UpdateDiceRollAll();
        }

        public void ResetDice()
        {
            bool allScored = true;
            foreach (Dice d in dices)
            {
                if (!d.IsScored)
                {
                    allScored = false;
                }
            }
            if (allScored)
            {
                foreach (Dice d in dices)
                {
                    d.SetPlayable();
                }
            }
        }

        //This loop handles all scoring in farkle
        public void ScoreDice(ref int runningScore)
        {
            int[] Counts = new int[7]; //0 Won't be used
            Console.WriteLine(runningScore);
            foreach (Dice d in dices)
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
                runningScore += 3000;
                return;
            }
            if (numTriples == 2)
            {
                runningScore += 2500;
                return;
            }
            if (FiveOfAKind > 0)
            {
                runningScore += 2000;
                //This loop removes scored dice
                //Not needed for Six of a Kind or 2 Triples
                for (int i = 0; i < 5;)
                {
                    foreach (Dice d in dices)
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
                    runningScore += 100;
                    //This loop removes scored dice
                    //Not needed for Six of a Kind or 2 Triples
                    for (int i = 0; i < 1;)
                    {
                        foreach (Dice d in dices)
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
                    runningScore += 50;
                    //This loop removes scored dice
                    //Not needed for Six of a Kind or 2 Triples
                    for (int i = 0; i < 1;)
                    {
                        foreach (Dice d in dices)
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
                runningScore += 1000;
                //This loop removes scored dice
                //Not needed for Six of a Kind or 2 Triples
                for (int i = 0; i < 4;)
                {
                    foreach (Dice d in dices)
                    {
                        if (!d.IsScored)
                        {
                            d.SetAside();
                            ++i;
                            break;
                        }
                    }
                }
                if (numPairs > 0)
                {
                    runningScore += 500;
                    //This loop removes scored dice
                    //Not needed for Six of a Kind or 2 Triples
                    for (int i = 0; i < 2;)
                    {
                        foreach (Dice d in dices)
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
                    runningScore += 100;
                    //This loop removes scored dice
                    //Not needed for Six of a Kind or 2 Triples
                    for (int i = 0; i < 1;)
                    {
                        foreach (Dice d in dices)
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
                    runningScore += 50;
                    //This loop removes scored dice
                    //Not needed for Six of a Kind or 2 Triples
                    for (int i = 0; i < 1;)
                    {
                        foreach (Dice d in dices)
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
                    runningScore += 300;
                }
                for (int i = 2; i < Counts.Length; ++i)
                {
                    if (Counts[i] == 3)
                    {
                        runningScore += i * 100;
                    }
                }
                //This loop removes scored dice
                //Not needed for Six of a Kind or 2 Triples
                for (int i = 0; i < 3;)
                {
                    foreach (Dice d in dices)
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
                    runningScore += 100 * Counts[1];
                    //This loop removes scored dice
                    //Not needed for Six of a Kind or 2 Triples
                    for (int i = 0; i < Counts[1];)
                    {
                        foreach (Dice d in dices)
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
                    runningScore += 50 * Counts[5];
                    //This loop removes scored dice
                    //Not needed for Six of a Kind or 2 Triples
                    for (int i = 0; i < Counts[5];)
                    {
                        foreach (Dice d in dices)
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
                runningScore += 1500;
                return;
            }

            if (Counts[1] == 1 && Counts[2] == 1 && Counts[3] == 1 && Counts[4] == 1 && Counts[5] == 1 && Counts[6] == 1)
            {
                runningScore += 1500;
                return;
            }

            //Farkle!
            if ((Counts[1] == 0 && Counts[5] == 0))
            {
                runningScore = 0;
                Console.WriteLine("Farkle!!");
                return;
            }

            if (Counts[1] > 0)
            {
                runningScore += 100 * Counts[1];

                //This loop removes scored dice
                //Not needed for Six of a Kind or 2 Triples
                for (int i = 0; i < Counts[1];)
                {
                    foreach (Dice d in dices)
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
                runningScore += 50 * Counts[5];

                //This loop removes scored dice
                //Not needed for Six of a Kind or 2 Triples
                for (int i = 0; i < Counts[5];)
                {
                    foreach (Dice d in dices)
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

        //Callbacks for updating players scores, showing the dice roll and allowing the game to start
        private void UpdateAllClientsGameStart()
        {
            foreach (ICallback cb in callbacks.Values)
                cb.UpdateGameStart(gameStart, callbacks.Keys.ElementAt(clientIndex));
        }

        private void UpdateAllClients()
        {
            foreach (ICallback cb in callbacks.Values)
                cb.Update(scores, callbacks.Keys.ElementAt(clientIndex), gameOver);
        }
        private void UpdateDiceRollAll() 
        {
            foreach (ICallback cb in callbacks.Values)
                cb.UpdateDiceRoll(roll);
        }
    }

}