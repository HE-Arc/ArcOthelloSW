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
            ApplicationManager.Instance.CurrentGame = new LocalGameHandler(BattleType.AgainstPlayer);

            // Init the game
            ApplicationManager.Instance.CurrentGame.Init("Me", "You");

            ApplicationManager.Instance.CurrentGame.Client.OnOrderReceived += OnOrderReceived;
            ApplicationManager.Instance.CurrentGame.Opponent.OnOrderReceived += OnOrderReceived;

            // Gameplay test
            ApplicationManager.Instance.CurrentGame.Client.Play('a', 0);

            // Wait
            Console.ReadLine();
        }

        private static void OnOrderReceived(object sender, OthelloTCPClientArgs e)
        {
            Console.WriteLine(e.Order.GetAcronym());
        }
    }
}
