
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
        //TODO Event key press

        public Lobby()
        {
            InitializeComponent();

            // Will listen for a gameStarted event
            ApplicationManager.Instance.Player1.OnGameStartedReceived += OnGameStartedReceived;
        }

        private void OnGameStartedReceived(object sender, OthelloTCPClientArgs e)
        {
            SwitchToGameboard();

            // Unsubscribe from the event
            ApplicationManager.Instance.Player1.OnGameStartedReceived -= OnGameStartedReceived;
        }

        private void SwitchToGameboard()
        {
            // TODO Bastien
        }

        private void OnStartGame(object sender, RoutedEventArgs e)
        {
            ApplicationManager.Instance.Player1.Ready();

            // Will be used if the game is local
            ApplicationManager.Instance.Player2?.Ready();
        }

    }
}
