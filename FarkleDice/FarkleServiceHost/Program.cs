/*
 * Program:     TestServiceHost.exe
 * Module:      Program.cs
 * Author:      T. Haworth
 * Date:        March 14, 2023
 * Description: Implements a service host for TestLibrary.Counting.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FarkleLibrary; // Service contract and implementation
using System.ServiceModel;  // WCF types

namespace TestServiceHost
{
    class Program
    {
        static void Main()
        {
            ServiceHost servHost = null;

            try
            {
                servHost = new ServiceHost(typeof(Counting));

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
