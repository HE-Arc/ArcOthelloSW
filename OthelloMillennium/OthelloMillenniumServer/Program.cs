using System;

namespace OthelloMillenniumServer
{
    class Program
    {
        static void Main(string[] args)
        {
            TCPServer serveur = TCPServer.Instance;
            serveur.StartListening();

            //Should be turned to deamon
            Console.ReadLine();
        }
    }
}
