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
            Client client1 = new Client(PlayerType.Human, "Me");
            Client client2 = new Client(PlayerType.Human, "eM");

            client1.OnGameStartedReceived += OnGameStartedReceived;

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
