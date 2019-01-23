
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
            ApplicationManager.Instance.Lobby = this;

            PlayerDataExport playersInfo = ApplicationManager.Instance.GetPlayers();

            string BlackPlayerName = playersInfo.Color1 == Color.Black ? playersInfo.Name1 : playersInfo.Name2;
            BlackPlayer.SetValue(PlayerVisualiserBlack.PropertyPseudo, BlackPlayerName);

            string WhitePlayerName = playersInfo.Color1 == Color.White ? playersInfo.Name1 : playersInfo.Name2;
            WhitePlayer.SetValue(PlayerVisualiserWhite.PropertyPseudo, WhitePlayerName);

            if (ApplicationManager.Instance.GameType == GameType.Local)
            {
                KeyUp += PlayerPicker.OnKeyDownLocal;
            }
            else
            {
                if(ApplicationManager.Instance.PlayersColor().Item1 == Color.Black)
                {
                    KeyUp += PlayerPicker.OnKeyDownOnlineBlack;
                }
                else
                {
                    KeyUp += PlayerPicker.OnKeyDownOnlineWhite;
                }
            }
            // Bind key event to playerpicker
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

        public void OnUpdateOpponentColorServer(Color color, int AvatarId)
        {
            PlayerPicker.OnUpdateOpponentColorServer(color, AvatarId);
            //TODO Update score interface
        }
    }
}
