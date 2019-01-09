using System;
using Tools;
using Tools.Classes;

namespace ClientTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // Init clients
            OthelloTCPClient c1 = new OthelloTCPClient();
            c1.OnOrderReceived += OthelloTCPClient_OnOrderReceived;
            c1.ConnectTo("127.0.0.1", 65432);
            
            OthelloTCPClient c2 = new OthelloTCPClient();
            c2.OnOrderReceived += OthelloTCPClient_OnOrderReceived;
            c2.ConnectTo("127.0.0.1", 65432);

            // Connect clients
            c1.Send(OrderProvider.SearchLocalGame);
            c2.Send(OrderProvider.SearchOnlineGame);

            // Wait
            Console.ReadLine();
        }

        private static void OthelloTCPClient_OnOrderReceived(object sender, OthelloTCPClientArgs e)
        {
            Console.WriteLine(e.Order.GetAcronym());
        }
    }
}
