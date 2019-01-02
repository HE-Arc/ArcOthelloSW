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
        private (int, int) playerOne;
        private (int, int) playerTwo;

        #endregion

        #region Properties
        public (int, int) PlayerOne
        {
            get => playerOne;
            set {
                playerOne = value;
                Grid.SetRow(SelectorPlayer1, value.Item1);
                Grid.SetColumn(SelectorPlayer1, value.Item2);
            }
        }

        public (int, int) PlayerTwo
        {
            get => playerTwo;
            set {
                playerTwo = value;
                Grid.SetRow(SelectorPlayer2, value.Item1);
                Grid.SetColumn(SelectorPlayer2, value.Item2);
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
            PlayerOne = (0, 0);
            PlayerTwo = (3, 4);

            DataContext = this;
        }

        // TODO Change selector image + different image for selectorOne and Two

        // Add Binding

    }
}
