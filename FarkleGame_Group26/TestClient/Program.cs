/*
 * Program:     Farkle.exe
 * Module:      Program.cs
 * Author:      Dustin Taylor, Hongseok Kim, Donghao Tang, Christopher Russell
 * Date:        March 30, 2023
 * Description: A console client for the Farkle service.  
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FarkleLibrary; // Service contract and implementation
using System.ServiceModel;  // WCF types

using System.Threading;
using System.Runtime.InteropServices;   // Need this for DllImport()

namespace FarkleClient
{
    class Program
    {
        private class CBObject : ICallback
        {
            //Handles game starting can not be played until atleast 2 players have joined
            public void UpdateGameStart(bool boolgamestart, int nextId) 
            {
                activeClientId = nextId;
                if (boolgamestart) 
                {
                    Console.WriteLine("The game can now begin!");
                }
                if (activeClientId == clientId) 
                {
                    Console.WriteLine("Press Enter when Ready to start.");
                    waitHandle.Set();
                    var key = Console.ReadKey();
                    if (key.Key == ConsoleKey.Enter) 
                    {
                        Update(new int[9], 1, false);
                    }
                }
            }
            public void Update(int[] scores, int nextId, bool over)
            {
                isGameStarted = true;
                activeClientId = nextId;
                gameOver = over;
                Console.WriteLine("Current Scores (Only Players who Have scored are shown)");
                for (int i = 0; i< scores.Length; ++i) 
                {
                    
                    if (scores[i] > 0)
                    {
                        Console.WriteLine($"Player {i}: {scores[i]} .");
                    }
                }
                if (gameOver)
                {
                    // Release all clients so they can exit out
                    for (int i = 0; i < scores.Length; ++i) 
                    {
                        if (scores[i] >= 2500) 
                        {
                            Console.WriteLine($"\n!$!$!$!Player {i} is the Winner!$!$!$!");
                        }
                    }
                    Console.WriteLine("*** The game is now over! Press any key to quit. ***");
                    waitHandle.Set();

                }
                else if (activeClientId == clientId)
                {
                    // Release this client's main thread to let this user "count"
                    Console.WriteLine("It's your turn. Press enter to play.");
                    waitHandle.Set();
                }
                else
                {
                    // Release this client's main thread to let this user "count"
                    Console.Write("It's not your turn.");
                    waitHandle.Reset();
                }
            }

            public void UpdateDiceRoll(string roll)
            {
                Console.WriteLine($"Player {activeClientId} Rolling: ...");
                Console.Write(roll+ "\n");
            }
        }

        private static IFarkle farkle = null;   // service object reference
        private static int clientId, activeClientId = 0;
        private static CBObject cbObj = new CBObject();
        private static EventWaitHandle waitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
        private static bool gameOver  = false;
        private static bool isGameStarted = false;
        private static bool registered = false;

        static void Main()
        {
            if (connect())
            {
                SetConsoleCtrlHandler(new HandlerRoutine(ConsoleCtrlCheck), true);

                do
                {
                    waitHandle.WaitOne();

                        if (gameOver)
                    {
                        Console.ReadKey();
                    }

                    else
                    {   /////////////////////////////////////////////////
                        //Game Logic
                        /////////////////////////////////////////////////
                  
                        bool isTurnOver = false;
                        int runningScore = 0;

                        //roll the dice until the player's turn is over
                        while (!isTurnOver && isGameStarted)
                        {
                            Console.Write($"Player {clientId} : Roll the dice! Press any key to start!");
                            var key = Console.ReadKey();


                            Console.WriteLine();
                            // Roll the dice
                            farkle.RollDice(clientId);

                            // Display the dice
                            farkle.DisplayDice();

                            // Determine the score for this roll
                            farkle.ScoreDice(ref runningScore);

                            Console.WriteLine($"Your running score: {runningScore}");
                            
                            //Farkle, the player can't accumulate the score and the turn has to be tossed
                            if (runningScore == 0)
                            {
                                Console.WriteLine($"Farkle on Player {clientId}. Not registered on the scoreboard.  No Points Scored.");
                                isTurnOver = true;
                                farkle.NextTurn();
                                waitHandle.Reset();
                            }
                            //If the running score is over 500 users can update the score
                            else if (runningScore >= 500 || registered)
                            {
                                //if there are one or more dice the user can roll the dice

                                farkle.ResetDice();
                                Console.WriteLine(farkle.PlayableDice());

                                Console.Write("\nWould you like to add your score(Ends turn)?: (Y/N) ");
                                key = Console.ReadKey();
                                Console.WriteLine();
                                if (key.Key == ConsoleKey.Y)
                                {   //🤡
                                    registered = true;
                                    farkle.UpdateScore(runningScore);
                                    runningScore = 0;
                                    isTurnOver = true;
                                    farkle.NextTurn();
                                    waitHandle.Reset();
                                }
                            }
                        }
                    }

                } while (!gameOver);

                farkle.LeaveGame();
            }
            else
            {
                Console.WriteLine("ERROR: Unable to connect to the service!");
            }
        }

        private static bool connect()
        {
            try
            {
                DuplexChannelFactory<IFarkle> channel = new DuplexChannelFactory<IFarkle>(cbObj, "FarkleEP");
                farkle = channel.CreateChannel();

                // Register for the callbacks (tells the Shoe object to include this instance of 
                // the client in future callback events (i.e. updates)
                clientId = farkle.JoinGame();

                Console.WriteLine("~ Welcome to Farkle! ~\n");

                if (clientId > 2)
                {
                    cbObj.UpdateGameStart(true, 1);

                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        private static bool ConsoleCtrlCheck(CtrlTypes ctrlType)
        {
            switch (ctrlType)
            {
                case CtrlTypes.CTRL_C_EVENT:
                case CtrlTypes.CTRL_BREAK_EVENT:
                    break;
                case CtrlTypes.CTRL_CLOSE_EVENT:
                case CtrlTypes.CTRL_LOGOFF_EVENT:
                case CtrlTypes.CTRL_SHUTDOWN_EVENT:
                    farkle?.LeaveGame();
                    break;
            }
            return true;
        }

        #region unmanaged

        // Declare the SetConsoleCtrlHandler function
        // as external and receiving a delegate.

        [DllImport("Kernel32")]

        public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);

        // A delegate type to be used as the handler routine
        // for SetConsoleCtrlHandler.
        public delegate bool HandlerRoutine(CtrlTypes CtrlType);

        // An enumerated type for the control messages
        // sent to the handler routine.
        public enum CtrlTypes
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT
        }

        #endregion

    }
}
