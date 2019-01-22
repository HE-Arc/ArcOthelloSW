using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    class OthelloPlayerServer : OrderHandler
    {

        // TODO Segan add function which can be send to the server
        
        public string Name { get; set; }
        public PlayerType PlayerType { get; set; }
        public Color Color { get; set; }

        private Client client;

        public OthelloPlayerServer(TcpClient tcpClient)
        {
            //TODO Think about receiving a client or a TcpClient
        }

        public void OpponentFound(string opponentName)
        {
            client.Send(new OpponentFoundOrder(opponentName));
        }

        public void OpponentAvatarChanged(int avatarId)
        {
            client.Send(new OpponentAvatarChangedOrder(avatarId));
        }

        public void HandleOrder(Order order)
        {
            switch (order)
            {
                case OpponentAvatarChangedOrder order:
                    OnOpponentAvatarChanged?.Invoke(this, e);
                    break;

                case RegisterSuccessfulOrder order:
                    semaphoreSearch.Release();
                    break;

                case GameStartedOrder order:
                    OnGameStartedReceived?.Invoke(this, e);
                    break;

                case GameReadyOrder order:
                    OnGameReadyReceived?.Invoke(this, e);
                    break;
            }
        }


    }
}
