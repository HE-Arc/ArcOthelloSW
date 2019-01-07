using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tools;

namespace OthelloMillenniumServer
{
    public class Client : OthelloTCPClient
    {
        public Client(string serverHostname, int serverPort)
            : base()
        {
            this.Connect(serverHostname, serverPort);
        }
    }
}
