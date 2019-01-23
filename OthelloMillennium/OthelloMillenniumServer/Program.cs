using System;

namespace OthelloMillenniumServer
{
    class Program
    {
        static void Main(string[] args)
        {
            TCPServer serveur = TCPServer.Instance;
            serveur.StartListening(Tools.GameType.Online);

            //Should be turned to deamon
            Console.ReadLine();
        }
    }
}
