using OthelloMillenniumClient.Classes;
using OthelloMillenniumClient.Classes.GameHandlers;
using System;
using System.Threading;
using System.Threading.Tasks;
using Tools;

namespace TestCommunication
{
    class Program
    {
        static void Main(string[] args)
        {
            Client_old client1 = new Client_old(PlayerType.Human, "Me");
            Client_old client2 = new Client_old(PlayerType.Human, "eM");

            client1.OnGameStartedReceived += OnGameStartedReceived;

            // Init the game
            new Task(() =>
            {
                // Register clients to applicationManager
                ApplicationManager.Instance.CurrentGame = new LocalGameHandler();
                ApplicationManager.Instance.CurrentGame.OnGameReady += OnGameReady;

                ApplicationManager.Instance.CurrentGame.Register(client1);
                ApplicationManager.Instance.CurrentGame.Register(client2);

                // TODO AVOID BUTTON SPAM
                ApplicationManager.Instance.CurrentGame.Search();

                // Register clients to applicationManager
                ApplicationManager.Instance.CurrentGame.Player1.Ready();
                ApplicationManager.Instance.CurrentGame.Player2.Ready();

            }).Start();

            // Wait
            Console.ReadLine();
        }

        private static void OnGameStartedReceived(object sender, OthelloTCPClientArgs e)
        {
            // Gameplay test
            ApplicationManager.Instance.CurrentGame.Place(new Tuple<char, int>('a', 0));
        }

        private static void OnGameReady(object sender, OthelloTCPClientArgs e)
        {
            Console.WriteLine("Game ready");            
        }
    }
}
