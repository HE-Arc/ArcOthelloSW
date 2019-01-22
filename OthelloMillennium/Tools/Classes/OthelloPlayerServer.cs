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

        /// <summary>
        /// Get : get the Name binded
        /// Set : set the Name and inform the server of the change
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Get : get the PlayerType
        /// Set : set the PlayerType
        /// </summary>
        public PlayerType PlayerType { get; set; }

        /// <summary>
        /// Get : get the Color
        /// Set : set the Color
        /// </summary>
        public Color Color { get; set; }

        public OthelloPlayerServer(TcpClient tcpClient)
        {

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
