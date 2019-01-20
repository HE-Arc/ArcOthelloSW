using OthelloMillenniumClient;
using OthelloMillenniumClient.Classes;
using OthelloMillenniumClient.Classes.GameHandlers;
using System;
using Tools;
using Tools.Classes;

namespace ClientTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // Init server and clients
            ApplicationManager.Instance.CurrentGame = new LocalGameHandler();

            Client client1 = new Client(PlayerType.Human, "Me");
            Client client2 = new Client(PlayerType.Human, "eM");

            ApplicationManager.Instance.CurrentGame.Register(client1);
            ApplicationManager.Instance.CurrentGame.Register(client2);

            // Init the game
            ApplicationManager.Instance.CurrentGame.Search();

            ApplicationManager.Instance.CurrentGame.Player1.OnOrderReceived += OnOrderReceived;
            ApplicationManager.Instance.CurrentGame.Player2.OnOrderReceived += OnOrderReceived;

            // Gameplay test
            ApplicationManager.Instance.CurrentGame.Player1.Play('a', 0);

            // Wait
            Console.ReadLine();
        }

        private static void OnOrderReceived(object sender, OthelloTCPClientArgs e)
        {
            Console.WriteLine(e.Order.GetAcronym());
        }
    }
}
