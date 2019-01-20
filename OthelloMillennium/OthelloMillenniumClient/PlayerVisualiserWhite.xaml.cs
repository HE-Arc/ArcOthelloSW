using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace OthelloMillenniumClient
{
    /// <summary>
    /// Logique d'interaction pour PlayerVisualiser.xaml
    /// </summary>
    public partial class PlayerVisualiserWhite : UserControl
    {
        #region Properties
        public string Image
        {
            get => (string)GetValue(PropertyImage);
            set
            {
                SetValue(PropertyImage, value);
            }
        }

        public string Pseudo
        {
            get => (string)GetValue(PropertyPseudo);
            set
            {
                SetValue(PropertyPseudo, value);
            }
        }

        public static readonly DependencyProperty PropertyImage
            = DependencyProperty.Register(
                  "Image",
                  typeof(string),
                  typeof(PlayerVisualiserWhite),
                  new PropertyMetadata("Images/None.png")
              );

        public static readonly DependencyProperty PropertyPseudo
            = DependencyProperty.Register(
                  "Pseudo",
                  typeof(string),
                  typeof(PlayerVisualiserWhite),
                  new PropertyMetadata("Player White")
              );
        #endregion

        public PlayerVisualiserWhite()
        {
            InitializeComponent();

            DataContext = this;
        }
    }
}
