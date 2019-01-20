
using OthelloMillenniumClient.Classes;
using System.Windows;


namespace OthelloMillenniumClient
{
    /// <summary>
    /// Logique d'interaction pour Saloon.xaml
    /// </summary>
    public partial class Lobby : Window
    {
        //TODO Event key press

        public Lobby()
        {
            InitializeComponent();
        }

        private void OnStartGame(object sender, RoutedEventArgs e)
        {
            ApplicationManager.Instance.Player1.Ready();

            // Will be used if the game is local
            ApplicationManager.Instance.Player2?.Ready();
        }

    }
}
