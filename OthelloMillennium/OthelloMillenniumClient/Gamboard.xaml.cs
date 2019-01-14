using OthelloMillenniumClient.Classes;
using System;
using System.Windows;
using System.Windows.Controls;
using Tools.Classes;

namespace OthelloMillenniumClient
{
    /// <summary>
    /// Logique d'interaction pour Gamboard.xaml
    /// </summary>
    public partial class Gamboard : UserControl
    {
        public Gamboard()
        {
            InitializeComponent();
        }

        private Client GetCurrentPlayer()
        {
            //TODO: Get Information from server or keep it local
            return null;
        }

        private void OnCellClick(object sender, RoutedEventArgs e)
        {
            char column = 'a';
            int row = 0;

            // Get the gamehandler
            IGameHandler gameHandler = ApplicationManager.Instance.CurrentGame;
            Client currentPlayer = gameHandler.GetCurrentPlayer();

            // Generate a new order
            var order = OrderProvider.PlayMove as PlayMoveOrder;
            order.Coords = new Tuple<char, int>(column, row);

            // Send the player new token location
            currentPlayer.Send(order);

            // End player's turn
            currentPlayer.Send(OrderProvider.NextTurn);
        }
    }
}

    
