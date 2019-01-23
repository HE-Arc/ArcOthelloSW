using OthelloMillenniumClient;
using System;
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
            ApplicationManager.Instance.JoinGameLocal(client1, client2);

            // Search, ready, etc
            //ApplicationManager.Instance.LaunchGame();

            // Wait
            Console.ReadLine();
        }
    }
}
