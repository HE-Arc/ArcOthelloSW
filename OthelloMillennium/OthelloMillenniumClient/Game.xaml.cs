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
using System.Windows.Shapes;

namespace OthelloMillenniumClient
{
    /// <summary>
    /// Logique d'interaction pour Game.xaml
    /// </summary>
    public partial class Game : Window
    {
        /// <summary>
        /// Should be binded to the gameboard in order to allow or not the player to play
        /// </summary>
        public bool Locked { get; private set; }

        public Game()
        {
            InitializeComponent();
            // ApplicationManager.Instance.Client.OnAwaitReceived += Client_OnAwaitReceived;
            // ApplicationManager.Instance.Client.OnBeginReceived += Client_OnBeginReceived;
        }

        private void Client_OnAwaitReceived(object sender, Tools.OthelloTCPClientArgs e)
        {
            Locked = true;
        }

        private void Client_OnBeginReceived(object sender, Tools.OthelloTCPClientArgs e)
        {
            Locked = false;
        }   
    }
}
