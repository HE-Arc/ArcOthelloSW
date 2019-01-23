using System.Windows;
using Tools;

namespace OthelloMillenniumClient
{
    /// <summary>
    /// Logique d'interaction pour EndGame.xaml
    /// </summary>
    public partial class EndGame : Window
    {
        public EndGame(bool victory, Color color, string name, int avatarId)
        {
            InitializeComponent();

            DataContext = this;

            GameResult.Content = victory ? "Victory !" : "Defeat !";
            if (color == Color.Black)
            {
                PlayerView.Child = new PlayerVisualiserBlack() {
                    Pseudo = name,
                    Image = AvatarSettings.IMAGE_FOLDER + AvatarSettings.IMAGES_PERSO[avatarId]
                };
            }
            else
            {
                PlayerView.Child = new PlayerVisualiserWhite()
                {
                    Pseudo = name,
                    Image = AvatarSettings.IMAGE_FOLDER + AvatarSettings.IMAGES_PERSO[avatarId]
                };
            }
        }

        private void OnHome(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Home home = new Home();
                home.Show();
                Close();
                (ApplicationManager.Instance.Game as Game).Close();
            });
        }

        private void OnClose(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
