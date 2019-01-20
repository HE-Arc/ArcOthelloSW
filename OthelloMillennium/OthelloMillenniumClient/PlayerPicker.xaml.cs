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
        private Tuple<int, int> playerOne;
        private Tuple<int, int> playerTwo;

        #endregion

        #region Properties
        public Tuple<int, int> PlayerOne
        {
            get => playerOne;
            set {
                playerOne = value;
                Grid.SetRow(BackgroundBlack, value.Item1);
                Grid.SetColumn(BackgroundBlack, value.Item2);
            }
        }

        public Tuple<int, int> PlayerTwo
        {
            get => playerTwo;
            set {
                playerTwo = value;
                Grid.SetRow(BackgroundWhite, value.Item1);
                Grid.SetColumn(BackgroundWhite, value.Item2);
            }
        }

        public string ImagePlayerOne
        {
            get { return (string)GetValue(ImagePlayerOneProperty); }
            set { SetValue(ImagePlayerOneProperty, value); }
        }

        public string ImagePlayerTwo
        {
            get { return (string)GetValue(ImagePlayerTwoProperty); }
            set { SetValue(ImagePlayerTwoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImagePlayerOne. This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImagePlayerOneProperty =
            DependencyProperty.Register("ImagePlayerOne", typeof(string), typeof(PlayerPicker), new PropertyMetadata("/Images/R2-D2.png"));
        public static readonly DependencyProperty ImagePlayerTwoProperty =
            DependencyProperty.Register("ImagePlayerTwo", typeof(string), typeof(PlayerPicker), new PropertyMetadata("/Images/Finn.png"));

        #endregion

        public PlayerPicker()
        {
            InitializeComponent();
            PlayerOne = new Tuple<int, int>(0, 0);
            PlayerTwo = new Tuple<int, int>(3, 4);

            DataContext = this;
        }

        private void Rectangle_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {

        }

        // TODO Change selector image + different image for selectorOne and Two

        // Add Binding

    }
}
