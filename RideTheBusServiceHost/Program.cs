using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

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
                //servHost = new ServiceHost(typeof(Shoe), new Uri("net.tcp://localhost:13200/CardsLibrary/"));

                // Register the service Contract and Binding
                //servHost.AddServiceEndpoint(typeof(IShoe), new NetTcpBinding(), "ShoeService");

                //servHost = new ServiceHost(typeof(Shoe));


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
