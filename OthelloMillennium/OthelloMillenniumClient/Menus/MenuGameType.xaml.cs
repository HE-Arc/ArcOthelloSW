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
using Tools;

namespace OthelloMillenniumClient
{
    /// <summary>
    /// Logique d'interaction pour MainOnline.xaml
    /// </summary>
    public partial class MenuGameType : UserControl
    {

        public event Action<GameType> GameTypeEvent;

        public void RaiseLocalEvent(object sender, System.Windows.RoutedEventArgs e)
        {
            // Your logic
            if (GameTypeEvent != null)
            {
                GameTypeEvent(GameType.Local);
            }
        }

        public void RaiseOnlineEvent(object sender, System.Windows.RoutedEventArgs e)
        {
            // Your logic
            if (GameTypeEvent != null)
            {
                GameTypeEvent(GameType.Online);
            }
        }

        public MenuGameType()
        {
            InitializeComponent();
        }
    }
}
