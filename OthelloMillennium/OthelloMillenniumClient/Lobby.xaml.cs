
using OthelloMillenniumClient.Classes;
using System;
using System.Windows;
using Tools;

namespace OthelloMillenniumClient
{
    /// <summary>
    /// Logique d'interaction pour Saloon.xaml
    /// </summary>
    public partial class Lobby : Window
    {
        public Lobby()
        {
            InitializeComponent();

            // Will listen for a gameStarted event
            //ApplicationManager.Instance.Player1.OnGameStartedReceived += OnGameStartedReceived;

            ApplicationManager.Instance.CurrentGame.OnGameReady += OnGameStartedReceived;

            // Bind key event to playerpicker
            this.KeyUp += PlayerPicker.OnKeyUpHandler;
        }

        private void OnGameStartedReceived(object sender, OthelloTCPClientArgs e)
        {
            SwitchToGameboard();

            // Unsubscribe from the event
            ApplicationManager.Instance.Player1.OnGameStartedReceived -= OnGameStartedReceived;
        }

        private void SwitchToGameboard()
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
            ApplicationManager.Instance.Player1.Ready();

            // Will be used if the game is local
            ApplicationManager.Instance.Player2?.Ready();
        }

    }
}
