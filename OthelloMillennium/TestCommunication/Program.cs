using OthelloMillenniumClient;
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
            OthelloPlayerClient client1 = new OthelloPlayerClient(PlayerType.Human, "Me");
            OthelloPlayerClient client2 = new OthelloPlayerClient(PlayerType.Human, "eM");

            // Register clients to applicationManager
            // ApplicationManager.Instance.JoinGameLocal()

            client1.Register();
            client2.Register();

            // TODO AVOID BUTTON SPAM
            client1.SearchOpponent(PlayerType.Human);
            client2.SearchOpponent(PlayerType.Human);

            // Register clients to applicationManager
            client1.ReadyToPlay();
            client2.ReadyToPlay();

            // Wait
            Console.ReadLine();
        }
    }
}
