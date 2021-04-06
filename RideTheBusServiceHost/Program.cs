/* Programmers: Colin Manliclic, Zina Long
 * Date:        April 9, 2021
 * Purpose:
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using RideTheBusLibrary;

namespace RideTheBusServiceHost
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost servHost = null;

            try
            {
                // Register the service Address
                //servHost = new ServiceHost(typeof(RideTheBus), new Uri("net.tcp://localhost:13200/RideTheBusLibrary/"));

                // Register the service Contract and Binding
                //servHost.AddServiceEndpoint(typeof(IRideTheBus), new NetTcpBinding(), "BusService");

                servHost = new ServiceHost(typeof(RideTheBus));


                // Run the service
                servHost.Open();

                Console.WriteLine("Service started. Press any key to quit.");
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
