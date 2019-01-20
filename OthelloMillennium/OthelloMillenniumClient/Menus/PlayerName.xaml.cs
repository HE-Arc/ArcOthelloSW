using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OthelloMillenniumClient
{
    /// <summary>
    /// Logique d'interaction pour PlayerName.xaml
    /// </summary>
    public partial class PlayerName : UserControl, Validable
    {

        #region Properties
        public string Pseudo
        {
            get => (string)GetValue(PropertyPseudo);
            set
            {
                SetValue(PropertyPseudo, value);
            }
        }

        public static readonly DependencyProperty PropertyPseudo
            = DependencyProperty.Register(
                  "Pseudo",
                  typeof(string),
                  typeof(PlayerCardBlack),
                  new PropertyMetadata("Player 1")
              );

        #endregion

        public PlayerName()
        {
            InitializeComponent();
        }

        public bool IsValid()
        {
            return Pseudo != "";
        }
    }
}
