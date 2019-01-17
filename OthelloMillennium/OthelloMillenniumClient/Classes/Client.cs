using OthelloMillenniumServer;
using System;
using Tools;
using Tools.Classes;

namespace OthelloMillenniumClient
{
    public class Client : OthelloTCPClient
    {
        public event EventHandler<OthelloTCPClientArgs> OnBeginReceived;
        public event EventHandler<OthelloTCPClientArgs> OnAwaitReceived;

        public Client(PlayerType type, GameType gameType)
            : base(type)
        {
            this.OnOrderReceived += Client_OnOrderReceived;

            switch (gameType)
            {
                case GameType.Local:
                    this.ConnectTo("localhost", TCPServer.Port);
                    break;
                case GameType.Online:
                    this.ConnectTo(OthelloMillenniumClient.Properties.Settings.Default.OnlineHostname, OthelloMillenniumClient.Properties.Settings.Default.OnlinePort);
                    break;
                default:
                    throw new Exception("Invalid gameType provided");
            }
        }

        private void Client_OnOrderReceived(object sender, OthelloTCPClientArgs e)
        {
            switch(e.Order)
            {
                case PlayerAwaitOrder order:
                    OnAwaitReceived?.Invoke(this, e);
                    break;
                case PlayerBeginOrder order:
                    OnBeginReceived?.Invoke(this, e);
                    break;
            }
        }

        /// <summary>
        /// Send a message to the binded server in order to register itself
        /// </summary>
        /// <param name="searchingFor">player type you're looking for</param>
        public void Search(PlayerType searchingFor)
        {
            switch(searchingFor)
            {
                case PlayerType.AI:
                    this.Send(new SearchBattleAgainstAIOrder()
                    {
                        PlayerType = this.Type
                    });
                    break;
                case PlayerType.Human:
                    this.Send(new SearchBattleAgainstPlayerOrder()
                    {
                        PlayerType = this.Type
                    });
                    break;
            }
        }

        /// <summary>
        /// Sends to the server the location where the new token has been placed
        /// </summary>
        /// <param name="row">row</param>
        /// <param name="column">column</param>
        public void Play(char row, int column)
        {
            this.Send(new PlayMoveOrder()
            {
                Coords = new Tuple<char, int>(row, column)
            });
        }
    }
}
