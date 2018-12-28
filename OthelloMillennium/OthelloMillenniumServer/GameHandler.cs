using System;
using System.Collections.Generic;
using System.Text;

namespace OthelloMillenniumServer
{
    class GameHandler
    {
        private readonly Client client1, client2;
        private readonly GameManager gameManager;

        public GameHandler(Client client1, Client client2)
        {
            this.client1 = client1;
            this.client2 = client2;

            //Init a gameManager
            this.gameManager = new GameManager();
        }
    }
}
