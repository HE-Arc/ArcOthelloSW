using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace OthelloMillenniumClient
{
    /// <summary>
    /// Logique d'interaction pour PlayerVisualiser.xaml
    /// </summary>
    public partial class PlayerVisualiser : UserControl
    {
        #region Properties
        public string ImagePlayer
        {
            get => (string)GetValue(PropertyImagePlayer);
            set
            {
                SetValue(PropertyImagePlayer, value);
                Debug.WriteLine(value);
            }
        }

        // Using a DependencyProperty as the backing store for Property1.  
        // This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PropertyImagePlayer
            = DependencyProperty.Register(
                  "ImagePlayer",
                  typeof(string),
                  typeof(PlayerVisualiser),
                  new PropertyMetadata("Images/None.png")
              );

        #endregion

        public PlayerVisualiser()
        {
            InitializeComponent();

            DataContext = this;
        }
    }
}
