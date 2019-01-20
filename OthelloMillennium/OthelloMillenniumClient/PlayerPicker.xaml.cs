using OthelloMillenniumClient.Classes;
using System;
using System.Windows;
using System.Windows.Controls;

namespace OthelloMillenniumClient
{
    /// <summary>
    /// Logique d'interaction pour Page1.xaml
    /// </summary>
    public partial class PlayerPicker : UserControl
    {

        #region Attributes
        private Tuple<int, int> playerBlack;
        private Tuple<int, int> playerWhite;

        #endregion

        #region Properties
        public Tuple<int, int> PlayerBlack
        {
            get => playerBlack;
            set {
                playerBlack = value;

                // TODO BASTIEN : peut-être à déplacer dans lobby ?
                // Utiliser la méthode ChangeAvatar d'un client lors d'un clic depuis l'interface sur un avatar pour en informer l'opposant
                Grid.SetRow(BackgroundBlack, value.Item1);
                Grid.SetColumn(BackgroundBlack, value.Item2);
            }
        }

        public Tuple<int, int> PlayerWhite
        {
            get => playerWhite;
            set {
                playerWhite = value;
                Grid.SetRow(BackgroundWhite, value.Item1);
                Grid.SetColumn(BackgroundWhite, value.Item2);
            }
        }

        public string ImagePlayerBlack
        {
            get { return (string)GetValue(ImagePlayerBlackProperty); }
            set { SetValue(ImagePlayerBlackProperty, value); }
        }

        public string ImagePlayerWhite
        {
            get { return (string)GetValue(ImagePlayerWhiteProperty); }
            set { SetValue(ImagePlayerWhiteProperty, value); }
        }

        public static readonly DependencyProperty ImagePlayerBlackProperty =
            DependencyProperty.Register("ImagePlayerBlack", typeof(string), typeof(PlayerPicker), new PropertyMetadata("/Images/R2-D2.png"));
        public static readonly DependencyProperty ImagePlayerWhiteProperty =
            DependencyProperty.Register("ImagePlayerWhite", typeof(string), typeof(PlayerPicker), new PropertyMetadata("/Images/Finn.png"));

        #endregion

        public PlayerPicker()
        {
            InitializeComponent();

            PlayerWhite = new Tuple<int, int>(3, 4);

            DataContext = this;

            //ApplicationManager.Instance.CurrentGame.Player1.ChangeAvatar
        }

        private void Rectangle_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {

        }

        // TODO Change selector image + different image for selectorOne and Two

        // Add Binding

    }
}
