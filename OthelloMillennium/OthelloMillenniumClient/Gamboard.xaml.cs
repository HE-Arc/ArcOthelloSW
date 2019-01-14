using OthelloMillenniumClient.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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

        private void OnCellClick(object sender, RoutedEventArgs e)
        {
            char column = 'a';
            int row = 0;

            // Generate a new order
            var order = OrderProvider.PlayMove as PlayMoveOrder;
            order.Coords = new Tuple<char, int>(column, row);

            // Send the player new token location
            ApplicationManager.Instance.Client.Send(order);

            // End player's turn
            ApplicationManager.Instance.Client.Send(OrderProvider.NextTurn);
        }
    }
}

    
