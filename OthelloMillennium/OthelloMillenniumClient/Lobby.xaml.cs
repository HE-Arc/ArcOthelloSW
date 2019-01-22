
using OthelloMillenniumClient.Classes;
using System;
using System.Windows;
using Tools;

namespace OthelloMillenniumClient
{
    /// <summary>
    /// Logique d'interaction pour Saloon.xaml
    /// </summary>
    public partial class Lobby : Window, ILobby
    {
        public Lobby()
        {
            InitializeComponent();

            // Bind key event to playerpicker
            this.KeyUp += PlayerPicker.OnKeyUpHandler;
        }

        public void OnLaunchGameServer()
        {
            Application.Current.Dispatcher.Invoke((Action)delegate {
                //Switch to the windows Game
                Game game = new Game();
                game.Show();
                this.Close();
            });
        }

        private void OnStartGame(object sender, RoutedEventArgs e)
        {
            ApplicationManager.Instance.LaunchGame();
        }

        public void OnUpdateOpponentColorServer()
        {

        }

    }
}
