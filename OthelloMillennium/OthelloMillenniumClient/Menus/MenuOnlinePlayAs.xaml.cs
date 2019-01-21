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
    /// Logique d'interaction pour MenuOnlinePlayAs.xaml
    /// </summary>
    public partial class MenuOnlinePlayAs : UserControl
    {

        public event Action<PlayerType> PlayerTypeEvent;

        public void RaiseHumanEvent(object sender, System.Windows.RoutedEventArgs e)
        {
            // Your logic
            if (PlayerTypeEvent != null)
            {
                PlayerTypeEvent(PlayerType.Human);
            }
        }

        public void RaiseAiEvent(object sender, System.Windows.RoutedEventArgs e)
        {
            // Your logic
            if (PlayerTypeEvent != null)
            {
                PlayerTypeEvent(PlayerType.AI);
            }
        }

        public MenuOnlinePlayAs()
        {
            InitializeComponent();
        }
    }
}
