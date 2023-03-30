/*
 * Program:     TestClient.exe
 * Module:      Program.cs
 * Author:      Dustin Taylor, Hongseok Kim, Donghao Tang, Christopher Russell
 * Date:        March 30, 2023
 * Description: Implements a service host for FarkleLibrary.Farkle.  
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FarkleLibrary; // Service contract and implementation
using System.ServiceModel;  // WCF types

namespace FarkleServiceHost
{
    class Program
    {
        static void Main()
        {
            ServiceHost servHost = null;

            try
            {
                servHost = new ServiceHost(typeof(Farkle));

                // Run the service
                servHost.Open();
                Console.WriteLine("Service started. Please any key to quit.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                // Wait for a keystroke
                Console.ReadKey();
                servHost?.Close();
            }
        }
    }
}
